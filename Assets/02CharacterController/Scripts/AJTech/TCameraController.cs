using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TCameraController : MonoBehaviour
{
    public bool lockCursor;
    public float mouseSensitivity = 2;
    public Transform robotTop;
    public Transform robotBody;
    public Transform robot;
    public float distFromTarget = 12f;
    public float distAboveTarget =10f;
    public Vector2 pitchMinMax = new Vector2(15, 20);
    public Vector3 camCurrentRotation = Vector3.zero;
    public Vector3 camTargetRotation;
    private Vector3 topCurrentRotation;
    private Vector3 bodyCurrentRotation;
    private Quaternion topPreviousRotation;
    public float camRotationDamping = 0.5f;//the greatet the faster Lerping
    private float topRotationDamping = 1f;
    public float bodyRotationDamping = 0.2f;

    private float yaw;
    private float pitch;

    // Use this for initialization
    void Start()
    {
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
        this.transform.eulerAngles = SmoothCamRotation(camTargetRotation);
        this.robotTop.eulerAngles = SmoothTopRotation(camCurrentRotation);
        this.robotBody.eulerAngles = SmoothBodyRotation(this.robotTop.eulerAngles);

        //remember top and body rotation 
        topCurrentRotation = this.robotTop.eulerAngles;
        bodyCurrentRotation = this.robotBody.eulerAngles;

        //apply rotation to parent (so as its Forward pointed correctly)
        robot.eulerAngles = this.robotBody.eulerAngles;

        //negate extra rotation of children caused by parent's rotation 
        this.robotTop.eulerAngles = topCurrentRotation;
        this.robotBody.eulerAngles = bodyCurrentRotation;

    }
    void LateUpdate()
    {
        this.transform.eulerAngles = SmoothCamRotation(camTargetRotation);
        this.ShiftCamBehind();
        topPreviousRotation = this.robotTop.rotation;
    }
    Vector3 TakeInputRotation()
    {
        yaw +=  Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);
        return new Vector3(pitch, yaw, 0);
    }
    Vector3 SmoothCamRotation(Vector3 targetRotation)
    {
        camCurrentRotation.y = Mathf.LerpAngle(camCurrentRotation.y, targetRotation.y, camRotationDamping *Time.deltaTime);
        camCurrentRotation.x = Mathf.LerpAngle(camCurrentRotation.x, targetRotation.x, camRotationDamping * Time.deltaTime); 
        return camCurrentRotation;
    }

    Vector3 SmoothTopRotation(Vector3 targetRotation)
    {
        return new Vector3(0, Mathf.LerpAngle(robotTop.eulerAngles.y, camCurrentRotation.y, topRotationDamping * (1 - Mathf.Exp(-20 * Time.deltaTime))), 0);
    }

    Vector3 SmoothBodyRotation(Vector3 targetRotation)
    {
        float bodyRotationY = Mathf.LerpAngle(robotBody.eulerAngles.y, robotTop.eulerAngles.y, bodyRotationDamping * (1 - Mathf.Exp(-20 * Time.deltaTime)));
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
    void ShiftCamBehind()
    {
        Vector3 tempCam = robotTop.position - transform.forward.normalized * distFromTarget;
        transform.position = new Vector3(tempCam.x, robotTop.position.y + distAboveTarget, tempCam.z);
    }
}
