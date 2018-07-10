using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotInput : MonoBehaviour {


    public float Horizontal { get; private set; }
    public float Vertical { get; private set; }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        Horizontal = Input.GetAxisRaw("Horizontal");
        Vertical = Input.GetAxisRaw("Vertical");

	}
}
