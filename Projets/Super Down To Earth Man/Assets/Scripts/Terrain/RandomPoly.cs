using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPoly : MonoBehaviour
{
    public float radius;
    public float verticesPerUnit;
    public float randomRange;
    public int smoothLevel;

    public void Set (float r, float f, float a, int s)
    {
        radius = r; verticesPerUnit = f; randomRange = a; smoothLevel = s;
    }

	public void Randomize ()
    {
        List<Vector2> randomPositions = RandomPositions();
        if (smoothLevel > 0) randomPositions = new List<Vector2>(Smooth(randomPositions.ToArray()));
        SetLine(randomPositions);
        SetMesh(randomPositions);
        SetCollider(randomPositions);
    }

    private void SetLine (List<Vector2> positions)
    {
        LineRenderer lr = GetComponent<LineRenderer>();
        if (lr == null) return;        
        lr.positionCount = positions.Count;
        lr.SetPositions(positions.ConvertAll(x => (Vector3)x).ToArray());        
    }

    private void SetMesh (List<Vector2> positions)
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf == null) return;

        mf.mesh = new Mesh();
        mf.sharedMesh.vertices = positions.ConvertAll(x => (Vector3)x).ToArray();
        Triangulator tr = new Triangulator(positions.ToArray());
        mf.sharedMesh.triangles = tr.Triangulate();
        mf.sharedMesh.RecalculateNormals();
        mf.sharedMesh.SetUVs(0, positions);
    }

    private void SetCollider(List<Vector2> positions)
    {
        PolygonCollider2D col = GetComponent<PolygonCollider2D>();
        if (col == null) return;
        col.points = positions.ToArray();
    }

    private List<Vector3> CirclePositions ()
    {
        List<Vector3> positions = new List<Vector3>();

        int nVertices = Mathf.FloorToInt(2f * Mathf.PI * verticesPerUnit);
        float aStep = 2 * Mathf.PI / (float)nVertices;
        for (int i = 0; i < nVertices; i++)
        {
            Vector3 u_pos = new Vector3(Mathf.Cos(i * aStep), Mathf.Sin(i * aStep));
            positions.Add(radius * u_pos);
        }

        return positions;
    }

    private List<Vector2> RandomPositions()
    {
        List<Vector2> positions = new List<Vector2>();

        int nVertices = Mathf.FloorToInt(2f * Mathf.PI * verticesPerUnit);
        float aStep = 2 * Mathf.PI / (float)nVertices;
        Vector3 prev_pos = radius * Vector3.right;

        for (int i = 1; i < nVertices; i++)
        {
            Vector3 pos = radius * new Vector3(Mathf.Cos(i * aStep), Mathf.Sin(i * aStep));
            Vector3 dpos = pos - prev_pos;
            dpos = Quaternion.Euler(0f, 0f, Random.Range(-randomRange, +randomRange)) * dpos;
            pos = prev_pos + dpos;
            positions.Add(pos);
            prev_pos = pos;
        }

        return positions;
    }

    private Vector2[] Smooth (Vector2[] positions)
    {
        if (smoothLevel <= 0) return positions;

        Vector2[] smoothedPositions = positions;

        for (int i = 0; i < smoothLevel; i++)
        {
            smoothedPositions = Subdivide(smoothedPositions);
        }

        return smoothedPositions;
    }

    private Vector2[] Subdivide(Vector2[] positions)
    {
        List<Vector2> newPositions = new List<Vector2>();
        for (int i = 0, imax = positions.Length; i < imax; i++)
        {
            Vector2[] sub = DivideAngle(positions[i], positions[(i + 1) % imax], positions[(i + 2) % imax]);
            newPositions.AddRange(sub);
        }
        return newPositions.ToArray();
    }

    private Vector2[] DivideAngle(Vector2 A, Vector2 B, Vector2 C)
    {
        Vector2[] output = new Vector2[2];
        output[0] = A / 4f + 3f * B / 4f;
        output[1] = 3f * B / 4f + C / 4f;

        return output;
    }
}