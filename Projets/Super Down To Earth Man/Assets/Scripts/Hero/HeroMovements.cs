using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HeroMovements : MonoBehaviour
{
    [Header("Basic moves")]
    public string groundTag = "Ground";
    public float speed;
    public float torque;
    public Vector2 jumpSpeed;

    [Header("Tumble")]
    public Trigger2DHelper feet;
    public float toleranceAngle;
    public float fallRoll;
    public float fallRollControl;
    public float controlledAngularDrag;
    public float fallingAngularDrag;
    public float tumbleDuration;

    [Header("Hurt")]
    public Vector2 hurtJumpSpeed;
    public float hurtSafeDuration;
    public float hurtBlinkSpeed;

    [Header("Animation")]
    public SpriteRenderer renderer;
    public Animator anim;
    public string jumpParameter;
    public string fallParameter;

    [HideInInspector] public float controlDirection;
    [HideInInspector] public bool controlFlip;

    private Rigidbody2D rb;
    private ItemReaction itemReact;

    public bool IsOnGround { get; private set; }
    public bool IsJumping { get; private set; }
    public bool IsFalling { get; private set; }

    private UnityAction<Vector3,int> actionOnHurt;

    void Start()
    {
        StartCoroutine(Init());
    }

    private IEnumerator Init()
    {
        rb = GetComponent<Rigidbody2D>();
        actionOnHurt = new UnityAction<Vector3,int>(HurtJump);

        itemReact = GetComponentInChildren<ItemReaction>();
        yield return new WaitUntil(() => itemReact.Initialized);
        itemReact.getsHurt.AddListener(actionOnHurt);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (IsFalling) return;

        Collider2D other = collision.collider;

        if (other.CompareTag(groundTag))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                Vector2 localTowardGround = Quaternion.Inverse(transform.rotation) * (contact.point - (Vector2)transform.position);
                float groundAngle = Vector2.Angle(Vector2.down, localTowardGround);
                if (groundAngle > 180f) groundAngle -= 180f;

                if (groundAngle < toleranceAngle)
                {
                    IsFalling = false;
                    rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                    rb.angularDrag = controlledAngularDrag;
                    transform.rotation = Quaternion.LookRotation(Vector3.forward, contact.normal);
                    anim.SetBool(fallParameter, false);
                }
                else
                {
                    StartCoroutine(TumbleCoroutine());
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (controlFlip) transform.Rotate(Vector3.up, 180f);

        if (IsFalling) return;

        IsOnGround = feet.IsColliding;
        Vector2 localVelocity = Quaternion.Inverse(transform.rotation) * rb.velocity;

        if (IsOnGround)
        {
            localVelocity = new Vector2(speed, localVelocity.y);

            if (controlDirection == 0f)
                IsJumping = false;
            else
            {
                if (!IsJumping)
                {
                    localVelocity += jumpSpeed;
                    IsJumping = true;
                }
            }
            anim.SetBool(jumpParameter, false);
        }
        else
        {
            IsJumping = true;
            if (controlDirection != 0f) rb.angularVelocity = torque * controlDirection;

            if (!IsFalling) anim.SetBool(jumpParameter, true);
        }

        rb.velocity = transform.rotation * localVelocity;

        rb.constraints = RigidbodyConstraints2D.None;
    }

    private void HurtJump(Vector3 hurtSource, int damage)
    {
        rb.velocity = Quaternion.LookRotation(Vector3.forward, transform.position - hurtSource) * hurtJumpSpeed;

        StartCoroutine(TumbleCoroutine());
        StartCoroutine(HurtCoroutine());
    }


    private IEnumerator TumbleCoroutine()
    {
        anim.SetBool(jumpParameter, false);
        anim.SetBool(fallParameter, true);

        rb.angularDrag = fallingAngularDrag;
        rb.constraints = RigidbodyConstraints2D.None;
        rb.AddTorque(fallRoll);

        if (IsFalling) yield return null;
        IsFalling = true;        

        float startTime = Time.time;
        while (Time.time < startTime + tumbleDuration)
        {
            rb.AddTorque(fallRollControl * controlDirection);
            yield return new WaitForFixedUpdate();
        }

        IsFalling = false;
    }

    private IEnumerator HurtCoroutine()
    {
        itemReact.gameObject.SetActive(false);

        float currentTime = Time.time;
        float startTime = Time.time;
        float blinkTime = startTime;

        while (currentTime < startTime + hurtSafeDuration)
        {
            if (currentTime > blinkTime + 2f / hurtBlinkSpeed)
            {
                renderer.color = new Color(0f, 0f, 0f, 0f);
                blinkTime = currentTime;
            }
            else if (currentTime > blinkTime + 1f / hurtBlinkSpeed)
                renderer.color = Color.white;

            yield return new WaitForFixedUpdate();
            currentTime = Time.time;
        }

        renderer.color = Color.white;
        itemReact.gameObject.SetActive(true);
    }
}
