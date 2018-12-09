using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalkCamera : MonoBehaviour
{
    public Transform target;
    public float distance;
    public float moveForce;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        rb.rotation = Quaternion.LookRotation(target.position - transform.position, Vector3.up);
        rb.velocity = moveForce * (target.position - target.rotation * Vector3.forward * distance - transform.position).normalized;
    }

}
