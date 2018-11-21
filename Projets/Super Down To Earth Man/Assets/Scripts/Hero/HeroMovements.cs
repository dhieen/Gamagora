using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroMovements : MonoBehaviour
{
    public string groundTag = "Ground";
    public float speed;
    public float torque;
    public Vector2 jumpSpeed;
    public float toleranceAngle;
    public float fallRoll;
    public float fallRollControl;
    public float controlledAngularDrag;
    public float fallingAngularDrag;

    [Header("Animation")]
    public Animator anim;
    public string jumpParameter;
    public string fallParameter;

    [HideInInspector] public float controlDirection;

    private Rigidbody2D rb;

    private bool isOnGround;
    private bool isJumping;
    private bool isFalling;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Collider2D other = collision.collider;

        if (other.CompareTag(groundTag))
        {
            isOnGround = true;

            foreach (ContactPoint2D contact in collision.contacts)
            {
                Vector2 localTowardGround = Quaternion.Inverse(transform.rotation) * (contact.point - (Vector2)transform.position);
                float groundAngle = Vector2.Angle(Vector2.down, localTowardGround);
                if (groundAngle > 180f) groundAngle -= 180f;

                if (groundAngle < toleranceAngle)
                {
                    isFalling = false;
                    StopCoroutine("FallCoroutine");
                    rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                    rb.angularDrag = controlledAngularDrag;
                    transform.rotation = Quaternion.LookRotation(Vector3.forward, contact.normal);
                    anim.SetBool(fallParameter, false);
                }
                else
                {
                    isFalling = true;
                    anim.SetBool(fallParameter, true);
                    rb.angularDrag = fallingAngularDrag;
                    rb.AddTorque(fallRoll);
                }
            }
        }
    }

    void FixedUpdate()
    {
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
            anim.SetBool(jumpParameter, false);
        }
        else
        {
            isJumping = true;
            if (controlDirection != 0f)
            {
                if (isFalling)
                    rb.AddTorque(fallRollControl * controlDirection);
                else
                    rb.angularVelocity = torque * controlDirection;
            }
            anim.SetBool(jumpParameter, true);
        }

        rb.velocity = transform.rotation * localVelocity;

        isOnGround = false;
        rb.constraints = RigidbodyConstraints2D.None;
    }
}
