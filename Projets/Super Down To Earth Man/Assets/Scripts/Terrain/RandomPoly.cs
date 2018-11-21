using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPoly
{
    private GameObject polyGO;
    private float radius;
    private float verticesPerUnit;
    private float randomRange;

    public RandomPoly (GameObject poly, float rad, float freq, float angle)
    {
        polyGO = poly;
        radius = rad;
        verticesPerUnit = freq;
        randomRange = angle;
    }

	public void Randomize ()
    {
        List<Vector2> randomPositions = RandomPositions();
        SetLine(randomPositions);
        SetMesh(randomPositions);
        SetCollider(randomPositions);
    }

    private void SetLine (List<Vector2> positions)
    {
        LineRenderer lr = polyGO.GetComponent<LineRenderer>();
        if (lr == null) return;        
        lr.positionCount = positions.Count;
        lr.SetPositions(positions.ConvertAll(x => (Vector3)x).ToArray());        
    }

    private void SetMesh (List<Vector2> positions)
    {
        MeshFilter mf = polyGO.GetComponent<MeshFilter>();
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
        PolygonCollider2D col = polyGO.GetComponent<PolygonCollider2D>();
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
}