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
                newCell.transform.localPosition = GetHexaPosition(x,y);
                newCell.coordinates = new Vector2Int(x, y);
                newCell.SetColor((x + y) % 2 == 0 ? color1 : color2);
                newCell.SetRenderOrder(size.y-y);
                newCell.name = "Cell (" + x + ", " + y + ")";

                if (x == 0 || x == size.x - 1 || x / 2 + y < (size.x) / 2 || x / 2 + y >= size.x - 1)
                {
                    newCell.SetWall(true);
                    newCell.SetColor(new Color(0f, 0f, 0f, 0f));
                    newCell.isStatic = true;
                }

                newColumn.Add(newCell as GameCell);
            }
            cells.Add(newColumn);
        }
    }

    private Vector2 GetHexaPosition (int x, int y)
    {
        return new Vector2 ((float)x * 1.5f * (1f/Mathf.Sqrt(3f)), (float)y + (float)x * .5f);
    }

    public override Vector2Int PositionToCoordinates(Vector2 pos)
    {
        return base.PositionToCoordinates(pos);
    }

    public override Vector2 CoordinatesToPosition(Vector2Int coordinates)
    {
        return GetHexaPosition (coordinates.x, coordinates.y);
    }
}
