using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public PrefabPicker groundPrefabs;
    public PrefabPicker airPrefabs;
    public float minSpace;

    public void SpawnAroundBloc (PolygonCollider2D bloc)
    {
        int pointCount = bloc.points.Length;
        Vector2 lastSpawnPos = float.MaxValue * Vector2.one;

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

            if (Vector2.Distance(lastSpawnPos, ptB) > minSpace)
            {
                GameObject prefab = airPrefabs.PickRandom();
                if (prefab != null)
                {
                    SpawnItemOn(prefab, ptB, medianOnB, bloc.transform);
                    lastSpawnPos = ptB;
                }

                if (Vector2.Distance(lastSpawnPos, (ptA + ptB) / 2f) > minSpace)
                {
                    prefab = groundPrefabs.PickRandom();
                    if (prefab != null)
                    {
                        SpawnItemOn(prefab, (ptA + ptB) / 2f, normalOnAB, bloc.transform);
                        lastSpawnPos = (ptA + ptB) / 2f;
                    }
                }
            }
        }
    }

    public void SpawnAlongTunnel (Tunnel tunnel)
    {
        int numSpaces = Mathf.FloorToInt(tunnel.segment.magnitude / minSpace);
        float step = tunnel.segment.magnitude / (float)numSpaces;

        for (int t =1 ; t < numSpaces; t++)
        {
            GameObject prefab = airPrefabs.PickRandom();
            if (prefab == null) continue;
            Vector2 pos = step * t * tunnel.segment.normalized;
            SpawnItemOn(prefab, pos, tunnel.GetNormal(), tunnel.transform);
        }

        Vector2[] seg = tunnel.GetCeilingSegment();
        numSpaces = Mathf.FloorToInt((seg[1] - seg[0]).magnitude / minSpace);
        step = (seg[1] - seg[0]).magnitude / (float)numSpaces;
        for (int t = 1; t < numSpaces; t++)
        {
            GameObject prefab = groundPrefabs.PickRandom();
            if (prefab == null) continue;
            Vector2 pos = seg[0] + step * t * (seg[1] - seg[0]).normalized;
            SpawnItemOn(prefab, pos, -tunnel.GetNormal(), tunnel.transform);
        }

        seg = tunnel.GetFloorSegment();
        numSpaces = Mathf.FloorToInt((seg[1] - seg[0]).magnitude / minSpace);
        step = (seg[1] - seg[0]).magnitude / (float)numSpaces;
        for (int t = 1; t < numSpaces; t++)
        {
            GameObject prefab = groundPrefabs.PickRandom();
            if (prefab == null) continue;
            Vector2 pos = seg[0] + step * t * (seg[1] - seg[0]).normalized;
            SpawnItemOn(prefab, pos, tunnel.GetNormal(), tunnel.transform);
        }
    }

    private void SpawnItemOn (GameObject prefab, Vector2 position, Vector2 axis, Transform onParent)
    {
        GameObject item = Instantiate<GameObject>(prefab, onParent);
        item.transform.localPosition = position;
        item.transform.rotation = Quaternion.LookRotation(Vector3.forward, axis);
    }
}