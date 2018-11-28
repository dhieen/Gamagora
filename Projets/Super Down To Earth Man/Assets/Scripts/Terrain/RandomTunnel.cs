using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTunnel : MonoBehaviour
{
    public Vector2 segment;
    public float width;

    public Vector2 GetNormal()
    {
        return new Vector2(-segment.y, segment.x).normalized;
    }

    public Geometry2D.Line GetCeilingLine ()
    {
        return new Geometry2D.Line(GetEnd(1) + GetNormal() * width / 2f, segment.normalized);
    }

    public Geometry2D.Line GetFloorLine()
    {
        return new Geometry2D.Line(GetEnd(1) - GetNormal() * width / 2f, segment.normalized);
    }

    public static Vector3[] Intersections (RandomTunnel A, RandomTunnel B, bool swapFloorAndCeiling = false)
    {
        Vector3[] intersections = new Vector3[2];

        if (!swapFloorAndCeiling)
        {
            intersections[0] = Geometry2D.Line.Intersection(A.GetCeilingLine(), B.GetCeilingLine());
            intersections[1] = Geometry2D.Line.Intersection(A.GetFloorLine(), B.GetFloorLine());
        }
        else
        {
            intersections[1] = Geometry2D.Line.Intersection(A.GetCeilingLine(), B.GetFloorLine());
            intersections[0] = Geometry2D.Line.Intersection(A.GetFloorLine(), B.GetCeilingLine());
        }

        return intersections;
    }

    public Vector2 GetEnd(int i)
    {
        LineRenderer[] lr = GetComponentsInChildren<LineRenderer>();
        return transform.position + (lr[0].GetPosition(i) + lr[1].GetPosition(i)) / 2f;
    }

    public void Set ()
    {
        SetLines(segment, width);
        SetColliders(segment, width);
    }

    public void LinkTo (ref RandomTunnel other)
    {
        bool sameWay = (Vector2.Dot(segment, other.segment) > 0f);
        int otherEndIndex = sameWay ? 1 : 0;
        Vector2 connectionPoint = other.GetEnd(otherEndIndex);

        transform.position = connectionPoint;
        Vector3[] intersections = Intersections(this, other, !sameWay);

        LineRenderer[] lr = GetComponentsInChildren<LineRenderer>();
        lr[sameWay ? 0 : 1].SetPosition (0, intersections[0] - transform.position);
        lr[sameWay ? 1 : 0].SetPosition(0, intersections[1] - transform.position);

        lr = other.GetComponentsInChildren<LineRenderer>();
        lr[0].SetPosition(otherEndIndex, intersections[0] - other.transform.position);
        lr[1].SetPosition(otherEndIndex, intersections[1] - other.transform.position);

        EdgeCollider2D[] col = GetComponentsInChildren<EdgeCollider2D>();
        col[sameWay ? 0 : 1].points = new Vector2[] { (Vector2)(intersections[0] - transform.position), col[sameWay ? 0 : 1].points[1] };
        col[sameWay ? 1 : 0].points = new Vector2[] { (Vector2)(intersections[1] - transform.position), col[sameWay ? 1 : 0].points[1] };

        col = other.GetComponentsInChildren<EdgeCollider2D>();
        col[0].points = new Vector2[]
        {
            sameWay ? col[0].points[0] : (Vector2)(intersections[0] - other.transform.position),
            sameWay ? (Vector2)(intersections[0] - other.transform.position) : col[0].points[1]
        };

        col[1].points = new Vector2[]
        {
            sameWay ? col[1].points[0] : (Vector2)(intersections[1] - other.transform.position),
            sameWay ? (Vector2)(intersections[1] - other.transform.position) : col[1].points[1]
        };

    }

    private void SetLines(Vector2 segment, float width)
    {
        LineRenderer[] lr = GetComponentsInChildren<LineRenderer>();
        if (lr == null || lr.Length != 2) return;

        Vector2 normal = GetNormal();

        lr[0].positionCount = 2;
        lr[0].SetPositions( new Vector3[]
        {
            normal * width/2f,
            segment + normal * width/2f
        });

        lr[1].positionCount = 2;
        lr[1].SetPositions(new Vector3[]
        {
            -normal * width/2f,
            segment - normal * width/2f
        });
    }
	
    private void SetColliders(Vector2 segment, float width)
    {
        EdgeCollider2D[] col = GetComponentsInChildren<EdgeCollider2D>();
        if (col == null || col.Length != 2) return;

        Vector2 normal = GetNormal();

        col[0].points = new Vector2[]
        {
            normal * width/2f,
            segment + normal * width/2f
        };

        col[1].points = new Vector2[]
        {
            -normal * width/2f,
            segment - normal * width/2f
        };
    }
}
