using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TCameraController : MonoBehaviour
{

    public float topRotY = 0;

    public bool lockCursor;
    public float mouseSensitivity = 2;
    public Transform robotTop;
    public Transform robotBody;
    public Transform robot;
    public float distFromTarget = 12f;
    public float distAboveTarget = 10f;
    public Vector2 pitchMinMax = new Vector2(15, 20);
    public Vector3 camCurrentRotation = Vector3.zero;
    public Vector3 camTargetRotation;
    private Vector3 topCurrentRotation;
    private Vector3 bodyCurrentRotation;
    private Quaternion topPreviousRotation;
    public float camRotationDamping = 0.5f;//the greatet the faster Lerping
    private float topRotationDamping = 1f;
    public float bodyRotationDamping = 0.2f;

    private RobotInput input;

    private float yaw;
    private float pitch;

    // Use this for initialization
    void Start()
    {
        input = this.GetComponent<RobotInput>();

        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
       
        camTargetRotation = this.TakeInputRotation();
        this.transform.eulerAngles = RotateCam(camTargetRotation);
        Vector3 tempCamCurrentRotation = RotateTop(camCurrentRotation);
       
        this.robotTop.eulerAngles = new Vector3(tempCamCurrentRotation.x, tempCamCurrentRotation.y, tempCamCurrentRotation.z + 270);
        RotateRobot();
        topRotY = this.robotTop.eulerAngles.y;

        //this.robotBody.eulerAngles = SmoothBodyRotation(this.robotTop.eulerAngles);
    }
    void LateUpdate()
    {
        // this.transform.eulerAngles = SmoothCamRotation(camTargetRotation);

        this.ShiftCamBehindTop();
        topPreviousRotation = this.robotTop.rotation;
    }
    Vector3 TakeInputRotation()
    {
        yaw += Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);
        return new Vector3(pitch, yaw, 0);
    }

    void RotateRobot()
    {
        //forward
        if (input.Vertical > 0 && input.Horizontal == 0)
        {
            RotateAllBodyParts(0, 0);
        }
        //left
        if (input.Horizontal < 0 && input.Vertical == 0)
        {
            RotateAllBodyParts(-90, 90);
        }
        //right
        if (input.Horizontal > 0 && input.Vertical == 0)
        {
            RotateAllBodyParts(90, -90);
        }

        //backward
        if (input.Vertical < 0 && input.Horizontal == 0)
        {
            RotateAllBodyParts(-180, 180);
        }
        //forward-left
        if (input.Vertical > 0 && input.Horizontal < 0)
        {
            float angle = Vector3.Angle(new Vector3(-1, 0, 0).normalized, new Vector3(input.Horizontal, 0, input.Vertical).normalized);
            RotateAllBodyParts(-angle, angle);
        }
        //forward-right
        if (input.Vertical > 0 && input.Horizontal > 0)
        {
            float angle = Vector3.Angle(new Vector3(1, 0, 0).normalized, new Vector3(input.Horizontal, 0, input.Vertical).normalized);
            RotateAllBodyParts(angle, -angle);
        }
        //backward-left
        if (input.Vertical < 0 && input.Horizontal < 0)
        {
            float angle = Vector3.Angle(new Vector3(0, 0, 1).normalized, new Vector3(input.Horizontal, 0, input.Vertical).normalized);
            RotateAllBodyParts(-angle, angle);
        }
        //backward-right
        if (input.Vertical < 0 && input.Horizontal > 0)
        {
            float angle = Vector3.Angle(new Vector3(0, 0, 1).normalized, new Vector3(input.Horizontal, 0, input.Vertical).normalized);
            RotateAllBodyParts(angle, -angle);
        }
    }

    void RotateAllBodyParts(float bodyTrailBehindDegree, float bodyRotationDegree)
    {
        topCurrentRotation = this.robotTop.eulerAngles;

        this.robotBody.eulerAngles = RotateBodyAccordingToTop(bodyTrailBehindDegree);

        this.robotTop.eulerAngles = topCurrentRotation;

        //remember top and body rotation  
        topCurrentRotation = this.robotTop.eulerAngles;
        bodyCurrentRotation = this.robotBody.eulerAngles;

        //apply rotation to parent (so as its Forward pointed correctly)
        robot.eulerAngles = new Vector3(this.robotBody.eulerAngles.x, this.robotBody.eulerAngles.y + bodyRotationDegree, this.robotBody.eulerAngles.z);

        //negate extra rotation of children caused by parent's rotation 
        this.robotBody.eulerAngles = bodyCurrentRotation;
        this.robotTop.eulerAngles = topCurrentRotation;
    }

    Vector3 RotateCam(Vector3 targetRotation)
    {
        camCurrentRotation.y = Mathf.LerpAngle(camCurrentRotation.y, targetRotation.y, camRotationDamping * Time.deltaTime);
        camCurrentRotation.x = Mathf.LerpAngle(camCurrentRotation.x, targetRotation.x, camRotationDamping * Time.deltaTime);
        return camCurrentRotation;
    }

    Vector3 RotateTop(Vector3 targetRotation)
    {
        return new Vector3(0, Mathf.LerpAngle(robotTop.eulerAngles.y, camCurrentRotation.y, topRotationDamping * (1 - Mathf.Exp(-20 * Time.deltaTime))), 0);
    }
    
    Vector3 RotateBodyAccordingToTop(float deg)
    {
        float bodyRotationY = Mathf.LerpAngle(robotBody.eulerAngles.y, robotTop.eulerAngles.y + deg, bodyRotationDamping * (1 - Mathf.Exp(-20 * Time.deltaTime)));
        //1 bodyRotationY = AjustForVeryFastTopRotation(bodyRotationY, targetRotation); //temporarly commented due to unexplainable rotation bug
        return new Vector3(0, bodyRotationY, 0);
    }

    float AjustForVeryFastTopRotation(float bodyRotationY, Vector3 targetRotation)
    {
        float y = Quaternion.Angle(robotTop.transform.rotation, robotBody.transform.rotation);
        if (y > 90)
        {
            Vector3 cross = Vector3.Cross(topPreviousRotation * Vector3.forward, this.robotTop.rotation * Vector3.forward);
            if (cross.y > 0) //turnRight  
            {
                bodyRotationY = targetRotation.y - 60f;
            }
            else
            {
                bodyRotationY = targetRotation.y + 60f;
            }
        }
        return y;
    }

    void ShiftCamBehindTop()
    {
        Vector3 tempCam = robotTop.position - transform.forward.normalized * distFromTarget;

        float camDeltaX = this.transform.eulerAngles.x <= 30f ? 30f - this.transform.eulerAngles.x : 30f + 360f - this.transform.eulerAngles.x;
        float aboveTarget = 8f - 8f / 50f * camDeltaX;

        transform.position = new Vector3(tempCam.x, robot.position.y + aboveTarget, tempCam.z);
    }
}
