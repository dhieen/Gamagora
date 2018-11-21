using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public SpawnableItem coinPrefab;
    public SpawnableItem hazardPrefab;

    public void SpawnAroundBloc (PolygonCollider2D bloc)
    {
        int pointCount = bloc.points.Length;

        for (int i= 0; i < pointCount; i++)
        {
            Vector2 ptA = bloc.points[i];
            Vector2 ptB = bloc.points[(i + 1) % pointCount];
            Vector2 ptC = bloc.points[(i + 2) % pointCount];

            Vector2 medianOnB = ((ptA - ptB).normalized + (ptC - ptB).normalized) / 2f;
            Vector2 normalOnAB = new Vector2(ptB.y - ptA.y, ptA.x - ptB.x).normalized;

            if (Vector2.SignedAngle(ptA - ptB, ptC - ptB) > 0f)
                medianOnB = medianOnB.normalized;
            else
                medianOnB = -medianOnB.normalized;

            SpawnItemOn(coinPrefab, ptB, medianOnB, bloc.transform);
            if (Vector2.Distance (ptA, ptB) > hazardPrefab.requiredSpace.x
                && Random.Range (0, 2) == 0)
                SpawnItemOn(hazardPrefab, (ptA+ptB)/2f, normalOnAB, bloc.transform);
        }
    }

    private void SpawnItemOn (SpawnableItem prefab, Vector2 position, Vector2 axis, Transform onParent)
    {
        SpawnableItem item = Instantiate<SpawnableItem>(prefab, onParent);
        item.transform.localPosition = position;
        item.transform.rotation = Quaternion.LookRotation(Vector3.forward, axis);
    }
}
