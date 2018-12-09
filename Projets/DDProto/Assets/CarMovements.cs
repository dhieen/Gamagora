using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovements : MonoBehaviour {
    
    public float motorTorque;
    public float steerAngle;
    public float driftTorque;
    public Vector3 dashVelocity;

    public Rigidbody carBody;
    public List<WheelCollider> motorWheels;
    public List<WheelCollider> steerWheels;

    private bool dashing;
	
	
	void FixedUpdate ()
    {
        float accelerate = Input.GetAxisRaw("Vertical");
        float steer = Input.GetAxisRaw("Horizontal");
        bool drift = Input.GetButton("Fire1");
        bool dash = Input.GetButtonDown("Fire2");

        foreach (WheelCollider wc in motorWheels)
        {
            wc.motorTorque = motorTorque * accelerate;
        }

        foreach (WheelCollider wc in steerWheels)
            wc.steerAngle = steer * steerAngle;

        if (drift)
            carBody.AddRelativeTorque(Vector3.up * driftTorque * steer);

        if (dash)
        {
            if (steer == 0f)
                carBody.velocity = carBody.velocity + carBody.rotation * new Vector3(0, dashVelocity.y, dashVelocity.z);
            else
                carBody.velocity = carBody.velocity + carBody.rotation * new Vector3(-steer * dashVelocity.x, dashVelocity.y, 0f);
        }
	}
}
