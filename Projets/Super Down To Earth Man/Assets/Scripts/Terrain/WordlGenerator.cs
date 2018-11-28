using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordlGenerator : MonoBehaviour
{
    public WorldCell cellPrefab;

    private Grid grid;
    private Dictionary<Vector2Int, GameObject> generated;

    public void GenerateCell(int cellX, int cellY)
    {
        if (grid == null) grid = GetComponent<Grid>();
        if (generated == null) generated = new Dictionary<Vector2Int, GameObject>();

        Vector2Int coordinates = new Vector2Int(cellX, cellY);
        WorldCell cell;

        if (generated.ContainsKey(coordinates))
        {
            if (generated[coordinates] != null)
            {
                cell = generated[coordinates].GetComponent<WorldCell>();
                if (cell == null)
                {
                    cell = Instantiate<WorldCell>(cellPrefab, this.transform);
                    cell.transform.localPosition = grid.CellToLocal(new Vector3Int(cellX, cellY, 0));
                }
            }
            else
            {
                cell = Instantiate<WorldCell>(cellPrefab, this.transform);
                cell.transform.localPosition = grid.CellToLocal(new Vector3Int(cellX, cellY, 0));
            }

            generated[coordinates] = cell.gameObject;
        }
        else
        {
            cell = Instantiate<WorldCell>(cellPrefab, this.transform);
            cell.transform.localPosition = grid.CellToLocal(new Vector3Int(cellX, cellY, 0));
            generated.Add(coordinates, cell.gameObject);
        }

        cell.GenerateContent(grid.cellSize, null);
    }

    private WorldCell GetCell(Vector2Int coordinates)
    {
        WorldCell cell;

        if (generated.ContainsKey(coordinates) && generated[coordinates] != null)
            cell = generated[coordinates].GetComponent<WorldCell>();
        else
            cell = null;

        return cell;
    }

    private List<WorldCell> GetNeighbours (Vector2Int coordinates)
    {
        List<WorldCell> nbc = new List<WorldCell>();
        WorldCell wc;

        wc = GetCell(coordinates + Vector2Int.right);
        if (wc != null) nbc.Add(wc);

        wc = GetCell(coordinates + Vector2Int.left);
        if (wc != null) nbc.Add(wc);

        wc = GetCell(coordinates + Vector2Int.down);
        if (wc != null) nbc.Add(wc);

        wc = GetCell(coordinates + Vector2Int.up);
        if (wc != null) nbc.Add(wc);

        return nbc;
    }
}
