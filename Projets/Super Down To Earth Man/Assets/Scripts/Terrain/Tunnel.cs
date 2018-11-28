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
        return new Geometry2D.Line((Vector2)transform.position + GetNormal() * width / 2f, segment.normalized);
    }

    public Geometry2D.Line GetFloorLine()
    {
        return new Geometry2D.Line((Vector2)transform.position - GetNormal() * width / 2f, segment.normalized);
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

        noIntersections = false;
        return intersections;
    }

    public Vector2 GetEnd(bool other)
    {
        return other ? (Vector2)transform.position : (Vector2)transform.position + segment;
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
        int thisEndIndex = connectionOrder ? ceiling.Count-1 : 0;
        int otherEndIndex = connectionOrder ? 0 : other.ceiling.Count-1;

        Vector2 connectionPoint = other.GetEnd(connectionOrder);
        transform.position = connectionOrder ? connectionPoint - segment : connectionPoint;

        bool noIntersections;
        Vector3[] intersections = Intersections(this, other, out noIntersections);
        
        if (noIntersections == false)
        {
            ceiling[thisEndIndex] = intersections[0] - transform.position;
            floor[thisEndIndex] = intersections[1] - transform.position;

            other.ceiling[otherEndIndex] = intersections[0] - other.transform.position;
            other.floor[otherEndIndex] = intersections[1] - other.transform.position;
        }
        else
        {
            if (thisEndIndex == 0)
            {
                ceiling.Insert(0, other.ceiling[otherEndIndex] + (Vector2)other.transform.position - (Vector2)transform.position);
                floor.Insert(0,other.floor[otherEndIndex] + (Vector2)other.transform.position - (Vector2)transform.position);
            }
            else
            {
                ceiling.Add(other.ceiling[otherEndIndex] + (Vector2)other.transform.position - (Vector2)transform.position);
                floor.Add(other.floor[otherEndIndex] + (Vector2)other.transform.position - (Vector2)transform.position);
            }
        }

        RefreshGameObject();
        other.RefreshGameObject();

        if (connectionOrder)
        {
            connections0.Add(other);
            other.connections1.Add(this);
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
        if (lr == null) return;

        lr[0].positionCount = ceiling.Count;
        lr[0].SetPositions(ceiling.ConvertAll (x => (Vector3)x).ToArray());
        lr[1].positionCount = ceiling.Count;
        lr[1].SetPositions(floor.ConvertAll(x => (Vector3)x).ToArray());
    }

    private void SetColliders()
    {
        EdgeCollider2D[] col = GetComponentsInChildren<EdgeCollider2D>();
        if (col == null) return;

        col[0].points = ceiling.ToArray();
        col[1].points = floor.ToArray();
    }

    private void SetMesh()
    {
        MeshFilter mf = GetComponentInChildren<MeshFilter>();
        if (mf == null) return;

        List<Vector2> quad = ceiling.Count <= 2 ?
            new List<Vector2>(new Vector2[4]
            {
                ceiling[0], ceiling[1], floor[1], floor[0]
            })
            :
            new List<Vector2>(new Vector2[4]
            {
                ceiling[1], ceiling[2], floor[2], floor[1]
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

        col.points = ceiling.Count <= 2 ?
            new Vector2[4]
            {
                ceiling[0], ceiling[1], floor[1], floor[0]
            }
            :
            new Vector2[4]
            {
                ceiling[1], ceiling[2], floor[2], floor[1]
            };
    }
}
