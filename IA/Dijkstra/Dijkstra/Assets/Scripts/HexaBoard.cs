using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HexaBoard : GameBoard {
    
    public float cellMetric = 1f;
    public bool flip90;   

    public override void InitCells()
    {
        cells = new List<List<GameCell>>();

        for (int x = 0; x < size.x; x++)
        {
            List<GameCell> newColumn = new List<GameCell>();
            for (int y = 0; y < size.y; y++)
            {
                Hexacell newCell = Instantiate<Hexacell>(cellPrefab as Hexacell, this.transform);
                newCell.Init();
                newCell.CellChangeEvent.AddListener(new UnityAction(OnCellChange));
                newCell.transform.localPosition = GetHexaPosition(x,y);
                newCell.SetColor((x + y) % 2 == 0 ? color1 : color2);
                newCell.name = "Cell (" + x + ", " + y + ")"; 
                newColumn.Add(newCell as GameCell);
            }
            cells.Add(newColumn);
        }

        SetStartEndLabels();
    }

    private Vector2 GetHexaPosition (int x, int y)
    {
        return new Vector2 ((float)x * 1.5f * (1f/Mathf.Sqrt(3f)), (float)y + (float)x * .5f);
    }
}
