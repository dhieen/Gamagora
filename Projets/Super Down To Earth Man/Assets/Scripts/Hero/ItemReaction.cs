using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ItemReaction : MonoBehaviour
{
    public class GetPointsEvent : UnityEvent<int,int> { }
    public class GetHurtEvent : UnityEvent<Vector3> { }

    public int health;
    public int score;

    public GetPointsEvent getsPoints;
    public GetHurtEvent getsHurt;
    public bool Initialized { get; private set; }

    private void Start()
    {
        getsPoints = new GetPointsEvent();
        getsHurt = new GetHurtEvent();
        Initialized = true;
    }

    public void GetPoints (int points)
    {
        score += points;
        getsPoints.Invoke(points, score);
    }

    public void GetHurt (Vector3 hurtSource)
    {
        health -= 1;
        getsHurt.Invoke(hurtSource);
    }
}
