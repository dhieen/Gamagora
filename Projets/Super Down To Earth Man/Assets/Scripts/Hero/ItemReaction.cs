using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ItemReaction : MonoBehaviour
{
    public class GetPointsEvent : UnityEvent<int,int> { }
    public class GetHurtEvent : UnityEvent<Vector3,int> { }

    public int health;
    public int score;
    public ParticleSystem coinParticles;

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

    public void GetHurt (Vector3 hurtSource, int damage)
    {
        int losePoint = Mathf.Min(damage, score);
        score -= losePoint;
        health -= 1;

        var emission = coinParticles.emission;
        emission.rateOverTime = losePoint * 1f / coinParticles.main.duration;
        coinParticles.Play();

        getsHurt.Invoke(hurtSource, losePoint);
        getsPoints.Invoke (-losePoint, score);
    }
}
