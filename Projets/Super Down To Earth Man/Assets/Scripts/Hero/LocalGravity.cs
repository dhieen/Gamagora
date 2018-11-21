using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalGravity : MonoBehaviour
{
    public float gravityForce;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    void FixedUpdate ()
    {
        rb.AddForce((transform.rotation * Vector2.down) * gravityForce);
	}
}
