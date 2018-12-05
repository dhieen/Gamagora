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

    public Vector2[] GetCeilingSegment()
    {
        if (ceiling.Count == 2) return ceiling.ToArray();

        if (Vector2.Angle (ceiling[1] - ceiling[0], segment) < 1f)
            return new Vector2[2] { ceiling[0], ceiling[1] };
        else
            return new Vector2[2] { ceiling[1], ceiling[2] };
    }

    public Vector2[] GetFloorSegment()
    {
        if (floor.Count == 2) return floor.ToArray();

        if (Vector2.Angle(floor[1] - floor[0], segment) < 1f)
            return new Vector2[2] { floor[0], floor[1] };
        else
            return new Vector2[2] { floor[1], floor[2] };
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
        Vector2 position2D = connectionOrder ? connectionPoint - segment : connectionPoint;
        transform.position = new Vector3(position2D.x, position2D.y, transform.position.z);

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
        List<MeshFilter> meshes = new List<MeshFilter> (GetComponentsInChildren<MeshFilter>());
        if (meshes == null || meshes.Count == 0) return;

        List<Vector2> vertices = new List<Vector2>();
        for (int i = 0; i < ceiling.Count; i++) vertices.Add(ceiling[i]);
        for (int i = 0; i < floor.Count; i++) vertices.Add(floor[floor.Count - 1 - i]);

        foreach (MeshFilter mf in meshes)
        {
            mf.mesh = new Mesh();
            mf.sharedMesh.vertices = vertices.ConvertAll(x => (Vector3)x).ToArray();
            Triangulator tr = new Triangulator(vertices.ToArray());
            mf.sharedMesh.triangles = tr.Triangulate();
            mf.sharedMesh.RecalculateNormals();
            mf.sharedMesh.SetUVs(0, vertices);
        }
    }

    private void SetTrigger()
    {
        PolygonCollider2D col = GetComponent<PolygonCollider2D>();
        if (col == null) return;

        List<Vector2> vertices = new List<Vector2>();
        for (int i = 0; i < ceiling.Count; i++) vertices.Add(ceiling[i]);
        for (int i = 0; i < floor.Count; i++) vertices.Add(floor[floor.Count - 1 - i]);

        col.points = vertices.ToArray();
    }

    public void SetCollisionsLayer (int layer)
    {
        gameObject.layer = layer;

        foreach (EdgeCollider2D c in new List<EdgeCollider2D>(GetComponentsInChildren<EdgeCollider2D>()))
            c.gameObject.layer = layer;
    }

    public void SetItemsActive (bool active)
    {
        foreach (ItemActivator item in new List<ItemActivator>(GetComponentsInChildren<ItemActivator>()))
        {
            item.SetInteractionActive(active);
        }
    }

    public void SetVisualLayer (float z)
    {        
        transform.position = new Vector3 (transform.position.x, transform.position.y, z);
    }
}
