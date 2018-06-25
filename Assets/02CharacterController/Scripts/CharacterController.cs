using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WR
{

    public class CharacterController : MonoBehaviour
    {

        [Serializable]
        public class MoveSettings
        {
            public float forwardVel = 3f;
            public float rotateVel = 50f;
            public float jumpVel = 15f;
            public float distToGrounded = 0.51f;
            public LayerMask ground; //used to determine what areas player is able to jump from
        }
        [Serializable]
        public class PhysSettings
        {
            public float downAccel = 0.7f;
        }
        [Serializable]
        public class InputSettings
        {
            public float inputDelay = 0.1f;
            public string FORWARD_AXIS = "Vertical";
            public string TURN_AXIS = "Horizontal";
            public string JUMP_AXIS = "Jump";
        }

        public MoveSettings moveSettings = new MoveSettings();
        public PhysSettings physSettings = new PhysSettings();
        public InputSettings inputSettings = new InputSettings();

        Vector3 velocity = Vector3.zero;
        Quaternion _targetRotation;
        public Quaternion TargetRotation
        {
            get
            {
                return _targetRotation;
            }
        }
        Rigidbody rBody;
        float forwardInput, turnInput, jumpInput;

        // Use this for initialization
        void Start()
        {

            _targetRotation = transform.rotation;
            rBody = GetComponent<Rigidbody>();
            forwardInput = turnInput = jumpInput = 0;

        }

        // Update is called once per frame
        void Update()
        {

        }

        float temp = -1;
        void FixedUpdate()
        {
            GetInput();
            Turn();
            Run();
           // Jump();

            rBody.velocity = transform.TransformDirection(velocity);
            if (temp != velocity.y)
            {
                Debug.Log(velocity.y);
            }
            temp = velocity.y;
            //rBody.velocity = velocity;
        }

        void GetInput()
        {
            forwardInput = Input.GetAxis(inputSettings.FORWARD_AXIS); //interpolated from -1 to 1
            turnInput = Input.GetAxis(inputSettings.TURN_AXIS); //interpolated from -1 to 1
            jumpInput = Input.GetAxisRaw(inputSettings.JUMP_AXIS); // not interpolated
        }

        void Run()
        {
            if (Mathf.Abs(forwardInput) > inputSettings.inputDelay)
            {
                this.velocity.z = forwardInput * moveSettings.forwardVel;
            }
            else
            {
                this.velocity.z = 0;
            }
        }

        void Turn()
        {

            if (Mathf.Abs(turnInput) > inputSettings.inputDelay)
            {
                _targetRotation *= Quaternion.AngleAxis(moveSettings.rotateVel * turnInput * Time.deltaTime, Vector3.up);

            }
            this.transform.rotation = this.TargetRotation;
        }

        void Jump()
        {
            if (jumpInput > 0 && Grounded())
            {
                velocity.y = moveSettings.jumpVel;
            }
            else if (jumpInput == 0 && Grounded())
            {
                velocity.y = 0;
            }
            else
            {
                velocity.y -= physSettings.downAccel;
            }
        }

        bool Grounded()
        {
            RaycastHit hit;
            bool b = Physics.Raycast(this.transform.position, Vector3.down, out hit, moveSettings.distToGrounded, moveSettings.ground);
            return b;
        }

    }
}
