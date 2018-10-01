using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameBoard : MonoBehaviour {

    protected List<List<GameCell>> cells;
    protected PathFinder pf;
    protected int currentView;

    public GameCell cellPrefab;
    public CellLabel labelPrefab;
    public Vector2Int size;
    public Color color1;
    public Color color2;
    public Color labelColor1;
    public Color labelColor2;
    public Vector2Int startCell;
    public Vector2Int endCell;
    public Text infoBox;
    public List<Vector2Int> directions = new List<Vector2Int> { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };

public void Start()
    {
        pf = new PathFinder { board = this };
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
                newCell.CellChangeEvent.AddListener(new UnityAction(OnCellChange));
                newCell.transform.localPosition = new Vector2(x, y);
                newCell.SetColor((x + y) % 2 == 0 ? color1 : color2);
                newCell.name = "Cell (" + x + ", " + y + ")";
                column.Add(newCell);
            }
            cells.Add(column);
        }

        SetStartEndLabels();
    }

    public void SetStartEndLabels()
    {
        CellLabel startLabel = Instantiate<CellLabel>(labelPrefab);
        startLabel.SetContent("A");
        startLabel.SetColor(labelColor1);
        PutLabelOn(startCell, startLabel);
        CellLabel endLabel = Instantiate<CellLabel>(labelPrefab);
        endLabel.SetContent("B");
        endLabel.SetColor(labelColor1);
        PutLabelOn(endCell, endLabel);
    }

    public GameCell GetCell(Vector2Int pos)
    {
        if (pos.x < size.x && pos.x >= 0 && pos.y < size.y && pos.y >= 0)
        {
            return cells[pos.x][pos.y];
        }

        return null;
    }

    private void PutLabelOn (Vector2Int pos, CellLabel label)
    {
        GameCell cell = GetCell(pos);
        if (cell == null)
        {
            return;
        }

        if (cell.GetComponentInChildren<CellLabel>() == null)
        {
            label.transform.parent = cell.transform;
            label.transform.localPosition = Vector2.zero;
        }
        else
        {
            cell.GetComponentInChildren<CellLabel>().AddContent(label.Content);
            Destroy(label.gameObject);
        }
    }

    private void ClearAllLabels ()
    {
        foreach (List<GameCell> column in cells)
            foreach(GameCell cell in column)
            {
                CellLabel label = cell.GetComponentInChildren<CellLabel>(true);
                if (label != null) DestroyImmediate(label.gameObject);
                
            }
        SetStartEndLabels();
    }

    private void PutLabelsOn(List<Vector2Int> positions, string content)
    {
        if (positions == null) return;

        for (int i = 0; i < positions.Count; i++)
        {
            CellLabel newLabel = Instantiate<CellLabel>(labelPrefab);
            newLabel.SetContent(content);
            newLabel.SetColor(labelColor2);
            PutLabelOn(positions[i], newLabel);
        }
    }

    private void PutLabelsOn(List<Vector2Int> positions, int startInt)
    {
        if (positions == null) return;

        for (int i = 0; i < positions.Count; i++)
        {
            CellLabel newLabel = Instantiate<CellLabel>(labelPrefab);
            newLabel.SetContent(((startInt + i)).ToString());
            newLabel.SetColor(labelColor2);
            PutLabelOn(positions[i], newLabel);
        }
    }

    private void PutLabelsOn (List<Vector2Int> positions, char content, bool increment = false)
    {
        if (positions == null) return;

        for (int i = 0; i < positions.Count; i++)
        {
            CellLabel newLabel = Instantiate<CellLabel>(labelPrefab);
            newLabel.SetContent(((char)(content + (increment ? i : 0))).ToString());
            newLabel.SetColor(labelColor2);
            PutLabelOn(positions[i], newLabel);
        }
    }   
    
    private void PutLabelsOn (List<Vector2Int> positions, string[] labelcontents, string defaultContent = "?")
    {
        for (int i = 0; i < positions.Count; i++)
        {
            CellLabel newLabel = Instantiate<CellLabel>(labelPrefab);
            newLabel.SetContent((i < labelcontents.Length ? labelcontents[i] : defaultContent).ToString());
            newLabel.SetColor(labelColor2);
            PutLabelOn(positions[i], newLabel);
        }
    }

    protected void OnCellChange()
    {
        ShowLabels(currentView);  
    }

    public void ShowLabels (int viewType)
    {
        ClearAllLabels();
        currentView = viewType;
        switch (currentView)
        {
            case 0:
                ShowGraph();
                break;
            case 1:
                ShowSubGraph();
                break;
            case 2:
                ShowShortestPath();
                break;
        }
    }

    public void ShowGraph()
    {
        List<PathFinder.PathStep> graph = pf.GetPathsGraph(startCell);
        List<Vector2Int> graphPositions = new List<Vector2Int>();
        foreach (PathFinder.PathStep s in graph)
        {
            if (s.pos != startCell && s.pos != endCell)
                graphPositions.Add(s.pos);
        }
        PutLabelsOn(graphPositions, 'C', true);

        infoBox.text = (graphPositions.Count + 2).ToString() + " nodes";
    }

    public void ShowSubGraph()
    {
        List<PathFinder.PathStep> graph = pf.GetPathsGraph(startCell);
        List<PathFinder.DijkstraStep> subGraph = pf.GetSubGraph(startCell, graph);
        List<Vector2Int> subGraphPositions = new List<Vector2Int>();
        List<string> subGraphLabels = new List<string>();
        foreach (PathFinder.DijkstraStep ds in subGraph)
        {
            subGraphPositions.Add(ds.pathStep.pos);
            subGraphLabels.Add(ds.weight.ToString());
        }
        if (subGraph != null)
        {
            PutLabelsOn(subGraphPositions, subGraphLabels.ToArray());
            infoBox.text = subGraph.Count.ToString() + " nodes";

            foreach (PathFinder.PathStep step in graph)
            {
                foreach (PathFinder.PathStep connected in step.reach)
                {
                    PutLabelsOn(GetCellLink(step.pos, connected.pos), ' ');
                }
            }
        }
        else
        {
            infoBox.text = ("no graph!");
        }

    }

    public void ShowShortestPath()
    {
        List<PathFinder.PathStep> graph = pf.GetPathsGraph(startCell);
        List<Vector2Int> shortestPath = pf.FindShorterPathFromTo(startCell, endCell, graph);
        
        if (shortestPath != null)
        {
            List<Vector2Int> links = new List<Vector2Int>();
            for (int i = 1; i < shortestPath.Count; i++)
            {
                List<Vector2Int> link = GetCellLink(shortestPath[i], shortestPath[i - 1]);
                if (link != null) foreach (Vector2Int x in link) links.Add(x);
            }
            PutLabelsOn(links, ' ');

            shortestPath.Remove(startCell);
            shortestPath.Remove(endCell);

            int stepCount = links.Count + shortestPath.Count;
            PutLabelsOn(shortestPath, 1);

            infoBox.text = "Shortest path: " + stepCount.ToString() + " steps";
        }
        else
        {
            infoBox.text = "No path!";
        }
    }

    public List<Vector2Int> GetCellLink(Vector2Int a, Vector2Int b)
    {
        if (a == b) return null;

        List<Vector2Int> link = new List<Vector2Int>();

        Vector2 directionf = ((Vector2)(b - a)).normalized;
        Vector2Int direction = new Vector2Int((int)directionf.x, (int)directionf.y);

        if (directions.Contains(direction) == false) return null;

        for (Vector2Int pos = a + direction; pos != b; pos += direction)
        {
            link.Add(pos);
        }
        return link;
    }
}
