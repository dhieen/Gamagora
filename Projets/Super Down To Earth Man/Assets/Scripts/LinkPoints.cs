using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkPoints : MonoBehaviour {

    public List<Transform> points;

    private LineRenderer lr;
    private MeshFilter mf;
    private int numPoints;
    private bool useLine;
    private bool useMesh;

    void Start ()
    {
        lr = GetComponent<LineRenderer>();
        mf = GetComponent<MeshFilter>();

        numPoints = points.Count;
        useLine = lr != null;
        useMesh = mf != null;

        if (useLine) lr.positionCount = numPoints;
        if (useMesh) mf.mesh = new Mesh();
    }	
	
	void Update ()
    {
        List<Vector2> positions = new List<Vector2>();
        foreach (Transform t in points) positions.Add(t.position - transform.position);
        Vector3[] positions3Array = positions.ConvertAll(x => (Vector3)x).ToArray();

        if (useLine)
        {
            lr.SetPositions(positions3Array );
        }

        if (useMesh)
        {
            for (int i = 0; i < numPoints; i++)
                mf.sharedMesh.vertices = positions3Array;

            Triangulator tr = new Triangulator(positions.ToArray());
            mf.sharedMesh.triangles = tr.Triangulate();
            mf.sharedMesh.RecalculateNormals();
            mf.sharedMesh.SetUVs(0, positions);
        }
    }
}
