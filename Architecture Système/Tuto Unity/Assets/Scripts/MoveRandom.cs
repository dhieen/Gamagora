using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class MoveRandom : MonoBehaviour
{
    public float speed = 2f;

    private Vector3 currentDestination;
    private Boundaries bound;

    void Start ()
    {
        bound = FindObjectOfType<Boundaries>();
        currentDestination = GetRandomPositionRange(bound.topLeftCorner.position, bound.bottomRightCorner.position);
    }

    void Update()
    {
        if (Vector3.Distance (transform.position, currentDestination) <= .1f)
        {
            currentDestination = GetRandomPositionRange(bound.topLeftCorner.position, bound.bottomRightCorner.position);
        }
        else
        {
            Vector3 movement = (currentDestination - transform.position).normalized;
            transform.position += movement * speed * Time.deltaTime;
        }        
    }

    private Vector3 GetRandomPositionRange(Vector3 topLeft, Vector3 bottomRight)
    {
        return new Vector3(Random.Range(topLeft.x, bottomRight.x), Random.Range(topLeft.y, bottomRight.y), Random.Range(topLeft.z, bottomRight.z));
    }
}
