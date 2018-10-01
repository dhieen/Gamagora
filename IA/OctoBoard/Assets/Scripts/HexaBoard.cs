using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexaBoard : MonoBehaviour {

    public Hexacell cellPrefab;
    public Vector2Int size;
    public float cellMetric = 1f;
    public bool flip90;

    private List<List<Hexacell>> cells;

    public void Start()
    {
        Build();
    }

    private void Build()
    {
        cells = new List<List<Hexacell>>();

        for (int x = 0; x < size.x; x++)
        {
            List<Hexacell> newColumn = new List<Hexacell>();
            for (int y = 0; y < size.y; y++)
            {
                Hexacell newCell = Instantiate<Hexacell>(cellPrefab, this.transform);
                newCell.transform.localPosition = GetHexaPosition(x,y);
                newCell.name = "Cell (" + x + ", " + y + ")"; 
                newColumn.Add(newCell);
            }
            cells.Add(newColumn);
        }
    }

    private Vector2 GetHexaPosition (int x, int y)
    {
        return new Vector2 ((float)x * 1.5f * (1f/Mathf.Sqrt(3f)), (float)y + (x%2==1 ? 0f : .5f));
    }
}
