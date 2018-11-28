using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tunnel : MonoBehaviour
{
    public Vector2 segment;
    public float width;
    public List<Tunnel> connections0;
    public List<Tunnel> connections1;

    public List<Vector2> ceiling;
    public List<Vector2> floor;

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

    public static Vector3[] Intersections (Tunnel A, Tunnel B, out bool noIntersections, bool swapFloorAndCeiling = false)
    {
        Vector3[] intersections = new Vector3[2];
        noIntersections = true;

        if (!swapFloorAndCeiling)
        {
            intersections[0] = Geometry2D.Line.Intersection(A.GetCeilingLine(), B.GetCeilingLine(), out noIntersections);
            if (noIntersections) return null;
            intersections[1] = Geometry2D.Line.Intersection(A.GetFloorLine(), B.GetFloorLine(), out noIntersections);
            if (noIntersections) return null;
        }
        else
        {
            intersections[0] = Geometry2D.Line.Intersection(A.GetCeilingLine(), B.GetFloorLine(), out noIntersections);
            if (noIntersections) return null;
            intersections[1] = Geometry2D.Line.Intersection(A.GetFloorLine(), B.GetCeilingLine(), out noIntersections);
            if (noIntersections) return null;
        }

        return intersections;
    }

    public Vector2 GetEnd(int i)
    {
        LineRenderer[] lr = GetComponentsInChildren<LineRenderer>();
        return transform.position + (lr[0].GetPosition(i) + lr[1].GetPosition(i)) / 2f;
    }

    public void RefreshGameObject()
    {
        SetLines();
        SetColliders();
        SetMesh();
        SetTrigger();
    }

    public void Set ()
    {
        Vector2 normal = GetNormal();

        ceiling = new List<Vector2> (new Vector2[2] { normal * width / 2f, segment + normal * width / 2f });
        floor = new List<Vector2> (new Vector2[2] {-normal * width/2f, segment - normal * width/2f });

        connections0 = new List<Tunnel> ();
        connections1 = new List<Tunnel>();

        RefreshGameObject();
    }

    public void LinkTo (ref Tunnel other, bool connectionOrder)
    {
        int thisEndIndex = connectionOrder ? 1 : 0;
        int otherEndIndex = connectionOrder ? 0 : 1;

        Vector2 connectionPoint = other.GetEnd(otherEndIndex);
        transform.position = connectionOrder ? connectionPoint - segment : connectionPoint;

        bool noIntersections;
        Vector3[] intersections = Intersections(this, other, out noIntersections);
        Debug.Assert(noIntersections == false);

        ceiling[thisEndIndex] = intersections[0] - transform.position;
        floor[thisEndIndex] = intersections[1] - transform.position;
        RefreshGameObject();

        other.ceiling[otherEndIndex] = intersections[0] - other.transform.position;
        other.floor[otherEndIndex] = intersections[1] - other.transform.position;
        other.RefreshGameObject();

        if (connectionOrder == false)
        {
            connections1.Add(other);
            other.connections0.Add(this);
        }
        else
        {
            connections1.Add(other);
            other.connections0.Add(this);
        }
    }

    private void SetLines()
    {
        LineRenderer[] lr = GetComponentsInChildren<LineRenderer>();
        if (lr == null || lr.Length != 2) return;

        lr[0].positionCount = 2;
        lr[0].SetPositions(ceiling.ConvertAll (x => (Vector3)x).ToArray());
        lr[1].positionCount = 2;
        lr[1].SetPositions(floor.ConvertAll(x => (Vector3)x).ToArray());
    }

    private void SetColliders()
    {
        EdgeCollider2D[] col = GetComponentsInChildren<EdgeCollider2D>();
        if (col == null || col.Length != 2) return;

        col[0].points = ceiling.ToArray();
        col[1].points = floor.ToArray();
    }

    private void SetMesh()
    {
        MeshFilter mf = GetComponentInChildren<MeshFilter>();
        if (mf == null) return;

        List<Vector2> quad = new List<Vector2>(new Vector2[4]
            {
                ceiling[0], ceiling[1], floor[1], floor[0]
            });

        mf.mesh = new Mesh();
        mf.sharedMesh.vertices = quad.ConvertAll (x => (Vector3)x).ToArray();
        Triangulator tr = new Triangulator(quad.ToArray());
        mf.sharedMesh.triangles = tr.Triangulate();
        mf.sharedMesh.RecalculateNormals();
        mf.sharedMesh.SetUVs(0, quad);
    }

    private void SetTrigger()
    {
        PolygonCollider2D col = GetComponent<PolygonCollider2D>();
        if (col == null) return;

        col.points = new Vector2[4]
            {
                ceiling[0], ceiling[1], floor[1], floor[0]
            };
    }
}
