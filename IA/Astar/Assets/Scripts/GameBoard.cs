using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{

    protected List<List<GameCell>> cells;

    public GameCell cellPrefab;
    public Vector2Int size;
    public Color color1;
    public Color color2;
    public List<Vector2Int> directions = new List<Vector2Int> { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };

    public void Start()
    {
        InitCells();
    }

    public virtual void InitCells()
    {
        cells = new List<List<GameCell>>();

        for (int x = 0; x < size.x; x++)
        {
            List<GameCell> column = new List<GameCell>();
            for (int y = 0; y < size.y; y++)
            {
                GameCell newCell = Instantiate<GameCell>(cellPrefab, this.transform);
                newCell.Init();
                newCell.transform.localPosition = new Vector2(x, y);
                newCell.SetColor((x + y) % 2 == 0 ? color1 : color2);
                newCell.name = "Cell (" + x + ", " + y + ")";
                column.Add(newCell);
            }
            cells.Add(column);
        }
    }

    public GameCell GetCell(Vector2Int pos)
    {
        if (pos.x < size.x && pos.x >= 0 && pos.y < size.y && pos.y >= 0)
        {
            return cells[pos.x][pos.y];
        }

        return null;
    }

    public List<GameCell> GetNeihbours(Vector2Int pos)
    {
        List<GameCell> nbrs = new List<GameCell>();

        foreach (Vector2Int d in directions)
        {
            GameCell n = GetCell(pos + d);
            if (n != null) nbrs.Add(n);
        }

        return nbrs;
    }

    public virtual Vector2Int PositionToCoordinates(Vector2 pos)
    {
        return new Vector2Int((int)(pos.x - transform.position.x), (int)(pos.y - transform.position.y));
    }

    public virtual Vector2 CoordinatesToPosition (Vector2Int coordinates)
    {
        return (Vector2)coordinates + (Vector2)transform.position;
    }

    public GameCell GetRandomCell (bool nowall, Vector2Int? avoid = null, float? radius = null)
    {
        List<GameCell> possibles = new List<GameCell>();
        for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
            {
                GameCell cell = GetCell(new Vector2Int (x, y));

                if ( !(nowall && cell.IsWall)
                    && !((avoid != null) && radius != null && Vector2Int.Distance (cell.coordinates, avoid.Value) < radius.Value))
                {
                    possibles.Add(cell);
                }
            }

        return possibles[Random.Range(0, possibles.Count)];
    }

    public IEnumerator ShockWave (Vector2Int origin, int radius, float speed)
    {
        for (int r = 0; r < radius; r++)
        {
            yield return new WaitForSecondsRealtime(1f / speed);
        }
    }
}
