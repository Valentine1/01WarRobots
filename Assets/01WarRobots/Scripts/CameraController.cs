using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public Transform target;
    public float lookSmooth = 0.09f;
    public Vector3 offsetFromTarget = new Vector3(0, 6, -8);
    public float xTilt = 15;


    Vector3 destination = Vector3.zero;
    CharacterController charController;
    public float rotateVel = 0;

	// Use this for initialization
	void Start () {
        charController = target.GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        MoveToTarget();
        LookAtTarget();
	}

    void LateUpdate()
    {
      
    }

    void MoveToTarget()
    {
        destination = target.position;
        destination += charController.TargetRotation * offsetFromTarget;
     
        transform.position = destination;
    }

    void LookAtTarget()
    {
        float eulerYAngel = Mathf.SmoothDampAngle(this.transform.eulerAngles.y, charController.TargetRotation.eulerAngles.y, ref rotateVel, lookSmooth);
        this.transform.rotation = Quaternion.Euler(xTilt, eulerYAngel, 0);
    }


}
