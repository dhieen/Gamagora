using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockBehaviour : MonoBehaviour
{

    public float minDistance;
    public float maxDistance;
    public float tooCloseWeight;
    public float tooFarWeight;
    public float velocityWeight;
    public float obstacleMinDistance;
    public float obstacleWeight;
    public string obstacleTag = "obstacle";
    public float densifyOnObstacle;

    private Boid controlledBoid;
    private List<Vector3> tooCloseInfluences;
    private List<Vector3> tooFarInfluences;
    private List<Vector3> velocityInfluences;
    private List<Vector3> obstacleInfluences;
    private List<GameObject> obstacles;

    private void Awake()
    {
        controlledBoid = GetComponent<Boid>();
        tooCloseInfluences = new List<Vector3>();
        tooFarInfluences = new List<Vector3>();
        velocityInfluences = new List<Vector3>();
        obstacleInfluences = new List<Vector3>();
        obstacles = new List<GameObject>();
    }

    private void FixedUpdate()
    {
        Vector3 averageInfluence = new Vector3();

        foreach (Vector3 inf in tooCloseInfluences)
            averageInfluence += tooCloseWeight * inf;
        foreach (Vector3 inf in tooFarInfluences)
            averageInfluence += tooFarWeight * inf;
        foreach (Vector3 inf in velocityInfluences)
            averageInfluence += velocityWeight * inf;
        foreach (Vector3 inf in obstacleInfluences)
            averageInfluence += obstacleWeight * inf;

        float denominator = tooCloseInfluences.Count * tooCloseWeight
                + tooFarInfluences.Count * tooFarWeight
                + velocityInfluences.Count * velocityWeight
                + obstacleInfluences.Count * obstacleWeight;

        if (denominator == 0f) return;

        averageInfluence /= denominator;
        controlledBoid.MoveToward(averageInfluence);

        tooCloseInfluences.Clear();
        tooFarInfluences.Clear();
        velocityInfluences.Clear();
        obstacleInfluences.Clear();
    }

    private void OnTriggerStay(Collider other)
    {
        Boid otherBoid = other.GetComponent<Boid>();

        if (otherBoid != null)
        {
            if (other.isTrigger && otherBoid.leaderFactor == 1f) return;

            Vector3 dPos = other.transform.position - this.transform.position;
            float distance = dPos.magnitude;
            float correctedMaxDistance = (obstacles.Count > 0) ? maxDistance / densifyOnObstacle : maxDistance;

            if (distance < minDistance)
            {
                if (distance == 0f) distance = float.MinValue;
                tooCloseInfluences.Add(-tooCloseWeight * minDistance * dPos.normalized / distance);
            }
            else if (distance > correctedMaxDistance)
            {
                tooFarInfluences.Add(tooFarWeight * otherBoid.leaderFactor * dPos.normalized * distance / correctedMaxDistance);
            }
            else
            {
                if (controlledBoid.Velocity.magnitude != 0f)
                    velocityInfluences.Add(velocityWeight * otherBoid.leaderFactor * otherBoid.Velocity / controlledBoid.Velocity.magnitude);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;

        if (other.tag == obstacleTag)
        {
            Ray ray = new Ray(transform.position, other.transform.position);
            RaycastHit rch = new RaycastHit();
            Physics.Raycast(ray, out rch);

            Vector3 dPos = rch.point - this.transform.position;
            float distance = dPos.magnitude;

            if (distance < obstacleMinDistance)
            {
                if (distance == 0f) distance = float.MinValue;
                tooCloseInfluences.Add(-obstacleWeight * obstacleMinDistance * dPos.normalized / distance);
            }

            if (obstacles.Contains(other.gameObject) == false) obstacles.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.isTrigger) return;

        if (other.tag == obstacleTag)
        {
            obstacles.Remove(other.gameObject);
        }
    }
}
        
