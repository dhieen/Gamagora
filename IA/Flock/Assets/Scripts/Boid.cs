using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour {

    public float acceleration;
    public float torque;
    public float leaderFactor = 1f;
    public float steerMargin = .2f;

    public Vector3 Velocity { get { return rb.velocity; } }
    public Quaternion Spin { get { return rb.rotation;  } }
    public float CurrentAcelerationPower { get; private set; }

    private Rigidbody rb;
    private IEnumerator accelerateCoroutine;
    private IEnumerator steerCoroutine;
    
    private DirectionUtility.Direction3 steerDirection;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        steerDirection = new DirectionUtility.Direction3();
        accelerateCoroutine = AccelerateCoroutine();
        steerCoroutine = SteerCoroutine();
    }

    public void AccelerateForward (float power = 1f)
    {
        CurrentAcelerationPower = power;
        rb.AddRelativeForce(Vector3.forward * acceleration * CurrentAcelerationPower);
    }

    public void AccelerateForward (bool start, float power = 1f)
    {
        CurrentAcelerationPower = power;

        if (start)
        {
            StartCoroutine(accelerateCoroutine);
        }
        else
        {
            StopCoroutine(accelerateCoroutine);
        }
    }

    private IEnumerator AccelerateCoroutine ()
    {
        while (true)
        {
            rb.AddRelativeForce(Vector3.forward * acceleration * CurrentAcelerationPower);
            yield return new WaitForFixedUpdate();
        }
    }

    public void VSteer (DIRECTION toward)
    {
        bool start = (toward != DIRECTION.ZERO && steerDirection.IsZero());
        steerDirection.y = toward;
        if (start) StartCoroutine(steerCoroutine);
        else if (steerDirection.IsZero()) StopCoroutine(steerCoroutine);
    }

    public void HSteer(DIRECTION toward)
    {
        bool start = (toward != DIRECTION.ZERO && steerDirection.IsZero());
        steerDirection.x = toward;
        if (start) StartCoroutine(steerCoroutine);
        else if (steerDirection.IsZero()) StopCoroutine(steerCoroutine);
    }

    public void ZSteer(DIRECTION toward)
    {
        bool start = (toward != DIRECTION.ZERO && steerDirection.IsZero());
        steerDirection.z = toward;
        if (start) StartCoroutine(steerCoroutine);
        else if (steerDirection.IsZero()) StopCoroutine(steerCoroutine);
    }

    private IEnumerator SteerCoroutine ()
    {
        while (true)
        {
            Vector3 steerVector = steerDirection.ToVector3();
            Vector3 torqueDirection = new Vector3(steerVector.y, steerVector.x, steerVector.z);

            rb.AddRelativeTorque (torque * torqueDirection);
            yield return new WaitForFixedUpdate();
        }
    }

    public void MoveToward(Vector3 direction) //,  Quaternion spin)
    {
        rb.AddForce(acceleration * Vector3.ClampMagnitude(direction, 1f));
    }
}
