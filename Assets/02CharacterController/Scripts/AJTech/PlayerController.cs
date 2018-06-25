using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [Header("Gizmos")]
    public GameObject sphere;
    public GameObject sphere2;
    public GameObject sphere3;
    public GameObject sphere4;


    public float movementSpeed;
    public float gravity = -2.5f;

    public float length;

    private Vector3 velocity;
    private Vector3 move;

    private bool grounded;
    private float currentGravity = 0;
    private CapsuleCollider playerCollider;
    // Use this for initialization
    void Start()
    {
       // playerCollider = this.GetComponent<CapsuleCollider>();
       // length = playerCollider.radius * 2;

    }

    // Update is called once per frame
    void Update()
    {
        SimpleMove();

        FinalMove();
        SlopeChecking();
    }
   
    void SimpleMove()
    {
        move = new Vector3(Input.GetAxis("Horizontal"), currentGravity, Input.GetAxis("Vertical"));
    }

    void FinalMove()
    {
        velocity = move * movementSpeed;
        velocity = transform.TransformDirection(velocity);
        this.transform.position += velocity * Time.deltaTime;
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

    void SlopeChecking()
    {
        Ray rayForward = new Ray(transform.position + transform.forward *1.88f, Vector3.down);
        Ray rayBack = new Ray(transform.position - transform.forward * 2.82f, Vector3.down);

        sphere.transform.position = rayForward.origin;
        sphere2.transform.position = rayBack.origin;

        RaycastHit hit1;
        RaycastHit hit2;
        Physics.Raycast(rayForward, out hit1,  15f);
        Physics.Raycast(rayBack, out hit2, 15f);

        sphere3.transform.position = hit1.point;
        sphere4.transform.position = hit2.point;

        if (hit1.point.y > hit2.point.y)
        {
        }
        else
        {

        }
        RaycastHit hit = hit1.point.y > hit2.point.y ? hit1 : hit2;
        this.transform.position = new Vector3(this.transform.position.x, hit.point.y + 5f, this.transform.position.z);
    }

}
