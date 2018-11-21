using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlMovements : MonoBehaviour
{
    public string groundTag = "Ground";
    public string controlAxisName = "Horizontal";
    public string jumpButtonName = "Jump";
    public float speed;
    public float torque;
    public Vector2 jumpSpeed;

    private Rigidbody2D rb;
    private bool isOnGround;
    private bool isJumping;

	void Start ()
    {
        rb = GetComponent<Rigidbody2D>();
	}

    private void OnCollisionStay2D(Collision2D collision)
    {
        Collider2D other = collision.collider;

        if (other.CompareTag(groundTag))
        {
            isOnGround = true;
        }
    }

    void FixedUpdate ()
    {
        rb.constraints = isOnGround ? RigidbodyConstraints2D.FreezeRotation : RigidbodyConstraints2D.None;

        float controlDirection = Input.GetAxisRaw(controlAxisName);
        
        Vector2 localVelocity = Quaternion.Inverse(transform.rotation) * rb.velocity;

        if (isOnGround)
        {
            localVelocity = new Vector2(speed, localVelocity.y);

            if (controlDirection == 0f)
                isJumping = false;
            else
            {
                if (!isJumping)
                {
                    localVelocity += jumpSpeed;
                    isJumping = true;
                }
            }
        }
        else
        {
            isJumping = true;
            if (controlDirection != 0f) rb.angularVelocity = torque * controlDirection;
        }            

        rb.velocity = transform.rotation * localVelocity;

        isOnGround = false;
	}
}
