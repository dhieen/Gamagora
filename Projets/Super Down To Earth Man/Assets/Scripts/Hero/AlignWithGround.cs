using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignWithGround : MonoBehaviour {

    public string groundTag = "Ground";
    public float toleranceAngle;
    public float fallBounce = 500f;
    public float fallRoll;

    [Header("Animation")]
    public Animator anim;
    public string fallParameter;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Collider2D other = collision.collider;

        if (other.CompareTag (groundTag))
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;

            foreach (ContactPoint2D contact in collision.contacts)
            {
                Vector2 localTowardGround = Quaternion.Inverse (transform.rotation) * (contact.point - (Vector2)transform.position);
                float groundAngle = Vector2.Angle(Vector2.down, localTowardGround);
                if (groundAngle > 180f) groundAngle -= 180f;

                if (groundAngle < toleranceAngle)
                {
                    transform.rotation = Quaternion.LookRotation (Vector3.forward, contact.normal);
                    anim.SetBool(fallParameter, false);
                }
                else
                {
                    rb.angularVelocity = fallRoll;
                    anim.SetBool(fallParameter, true);
                }
            }
        }
    }
}
