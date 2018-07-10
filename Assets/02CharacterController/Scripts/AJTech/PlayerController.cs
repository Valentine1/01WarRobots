#define TestMoveRays
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
     [System.Serializable]
    public struct RayCastObjects
    {
         [SerializeField]
         internal GameObject sphMiddle;
         [SerializeField]
        internal GameObject sphFront;
         [SerializeField]
        internal GameObject sphBack;
         [SerializeField]
        internal GameObject sphRight;
         [SerializeField]
        internal GameObject sphLeft;
         [SerializeField]
        internal GameObject sphMiddleCast;
         [SerializeField]
        internal GameObject sphFrontCast;
         [SerializeField]
        internal GameObject sphBackCast;
         [SerializeField]
        internal GameObject sphRightCast;
         [SerializeField]
        internal GameObject sphLeftCast;
    }

     [System.Serializable]
    public class RayCastOffsets
     {
         [SerializeField]
         internal float Mdl = 4.85f;
         [SerializeField]
         internal float Fwd = 1.5f;
         [SerializeField]
         internal float Bckd = 1.7f;
         [SerializeField]
         internal float Right = 1.5f;
         [SerializeField]
         internal float Left = 1.5f;
     }

    [SerializeField]
    private RayCastObjects rayCasts = new RayCastObjects();
    [SerializeField]
    private RayCastOffsets rayOffsets = new RayCastOffsets();

    public float movementSpeed;
    public float gravity = -2.5f;

    public float length;

    private Vector3 velocity;
    private Vector3 move;

    private RobotInput input;

    private bool grounded;
    private float currentGravity = 0;
    private CapsuleCollider playerCollider;
    private Rigidbody rb;
    private bool isInAir;
    private float previousFrameHitY;
    private string lastMoveState;
    // Use this for initialization
    void Start()
    {
        input = this.GetComponent<RobotInput>();
       // playerCollider = this.GetComponent<CapsuleCollider>();
       // length = playerCollider.radius * 2;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        SimpleMove();
        FinalMove();
        SlopeChecking();
    }
   
    void SimpleMove()
    {
        move = new Vector3(input.Horizontal, currentGravity, input.Vertical);
    }

    void FinalMove()
    {
        velocity = move * movementSpeed;
        velocity = transform.TransformDirection(velocity);
        rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        //this.transform.position += velocity * Time.deltaTime;
    }

    void ApplyGravity()
    {
        if (!grounded)
        {
            currentGravity = gravity;
        }
        else
        {
            currentGravity = 0;
        }
    }

    private void DrawRayGizmos(Probe rayMiddle,Probe rayForward,Probe rayBack,Probe rayRight,Probe rayLeft)
    {
        rayCasts.sphMiddle.transform.position = rayMiddle.hitRay.origin;
        rayCasts.sphFront.transform.position = rayForward.hitRay.origin;
        rayCasts.sphBack.transform.position = rayBack.hitRay.origin;
        rayCasts.sphRight.transform.position = rayRight.hitRay.origin;
        rayCasts.sphLeft.transform.position = rayLeft.hitRay.origin;

        rayCasts.sphMiddleCast.transform.position = rayMiddle.hitResult.point;
        rayCasts.sphFrontCast.transform.position = rayForward.hitResult.point;
        rayCasts.sphBackCast.transform.position = rayBack.hitResult.point;
        rayCasts.sphRightCast.transform.position = rayRight.hitResult.point;
        rayCasts.sphLeftCast.transform.position = rayLeft.hitResult.point;
    }

    void SlopeChecking()
    {
        Vector3 downOffset = transform.position - transform.up * rayOffsets.Mdl;

        Probe rayMiddle = MakeRaycast(downOffset);
        Probe rayForward = MakeRaycast(downOffset + transform.forward * rayOffsets.Fwd);
        Probe rayBack = MakeRaycast(downOffset - transform.forward * rayOffsets.Bckd);
        Probe rayRight = MakeRaycast(downOffset + transform.right * rayOffsets.Right);
        Probe rayLeft = MakeRaycast(downOffset  - transform.right * rayOffsets.Left);

        #if TestMoveRays
        DrawRayGizmos(rayMiddle, rayForward, rayBack, rayRight, rayLeft);
        #endif
      
        isInAir = rayMiddle.hitResult.distance > 0.2f;
        if (previousFrameHitY - rayMiddle.hitResult.point.y <= 0.2)
        {
            rb.MovePosition(new Vector3(rb.position.x, rayMiddle.hitResult.point.y + 5f, rb.position.z));
            previousFrameHitY = rayMiddle.hitResult.point.y;
            LogState("Moving");
        }
        else
        {
            if (rayMiddle.hitResult.distance > 0.2 && rayForward.hitResult.distance > 0.2
                && rayBack.hitResult.distance > 0.2
                && rayRight.hitResult.distance > 0.2
                && rayLeft.hitResult.distance > 0.2)
            {
                Landing();
                previousFrameHitY -= 10 * Time.fixedDeltaTime;
                LogState("Falling");
            }
            else
            {
                LogState("About to fall");
            }
        }
     
       // this.transform.position = new Vector3(this.transform.position.x, hit.point.y + 5f, this.tranSsform.position.z);
    }

    private void LogState(string state)
    {
        if (lastMoveState != state)
        {
            Debug.Log(state);
            Debug.Log(previousFrameHitY);
            lastMoveState = state;
        }
    }
    private void Landing()
    {
        float f =rb.position.y - 10 * Time.fixedDeltaTime;
        rb.MovePosition(new Vector3(rb.position.x, f, rb.position.z));
    }

    private Probe MakeRaycast(Vector3 origin){
        Probe p = new Probe();
        p.hitRay = new Ray(origin,Vector3.down);
        Physics.Raycast(p.hitRay, out p.hitResult, 100f);
        return p;
    }

}

public struct Probe 
{

    public Ray hitRay;

    public RaycastHit hitResult;

}
