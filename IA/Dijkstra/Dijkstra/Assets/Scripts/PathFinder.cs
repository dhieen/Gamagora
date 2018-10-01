using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder {

    public class PathStep
    {
        public Vector2Int pos;
        public List<PathStep> reach;

        static public int Distance (PathStep a, PathStep b)
        {
            return (int) Vector2Int.Distance(a.pos, b.pos);
        }
    }

    public class DijkstraStep
    {
        public PathStep pathStep;
        public int weight;
        public DijkstraStep prev;
    }

    public GameBoard board;

    public List<Vector2Int> FindReachExcluding(Vector2Int from, Vector2Int excludePos)
    {
        List<Vector2Int> reach = new List<Vector2Int>();        

        foreach (Vector2Int dir in board.directions)
        {
            Vector2Int pos = from + dir;

            if (board.GetCell(pos) != null && board.GetCell(pos).IsWall == false)
            {
                while (board.GetCell(pos + dir) != null
                    && board.GetCell(pos + dir).IsWall == false
                    && pos != board.endCell)
                {
                    pos += dir;
                }

                if (pos != excludePos) reach.Add(pos);
            }                
        }

        return reach;
    }

    public List<Vector2Int> FindReach(Vector2Int from)
    {
        return FindReachExcluding(from, new Vector2Int(-1, -1));
    }

    public List<PathStep> GetPathsGraph (Vector2Int from)
    {
        List<PathStep> pathsGraph = new List<PathStep>();
        PathStep start = new PathStep { pos = from, reach = new List<PathStep>() };
        pathsGraph.Add(start);

        BuildPathsGraphFrom(ref start, ref pathsGraph, new Vector2Int(-1, -1));

        return pathsGraph;
    }

    private void BuildPathsGraphFrom (ref PathStep from, ref List<PathStep> pathsGraph, Vector2Int previousPos)
    {
        List<Vector2Int> reachablePositions = FindReachExcluding(from.pos, previousPos);

        foreach (Vector2Int position in reachablePositions)
        {
            PathStep nextStep = pathsGraph.Find(p => p.pos == position);

            if (nextStep == null)
            {
                nextStep = new PathStep { pos = position, reach = new List<PathStep>() };
                pathsGraph.Add(nextStep);
                BuildPathsGraphFrom(ref nextStep, ref pathsGraph, from.pos);
            }
            
            from.reach.Add(nextStep);
        }
    }

    public List<DijkstraStep> GetSubGraph (Vector2Int from, List<PathStep> graph)
    {
        // SubGraph and a copy to keep track of processed elements
        List<DijkstraStep> subGraph = new List<DijkstraStep>();
        List<DijkstraStep> unprocessed = new List<DijkstraStep>();

        // Subgraphes initialization
        foreach (PathStep step in graph)
        {
            DijkstraStep ds = new DijkstraStep { pathStep = step, weight = (step.pos == from) ? 0 : int.MaxValue, prev = null };
            subGraph.Add(ds);
            unprocessed.Add(ds);
        }

        // Computes subgraph values: shorter distances, previous element
        while (unprocessed.Count > 0)
        {
            DijkstraStep current = new DijkstraStep { pathStep = null, prev = null, weight = int.MaxValue };
            foreach (DijkstraStep ds in unprocessed)
                if (ds.weight < current.weight) current = ds;

            foreach (PathStep next in current.pathStep.reach)
            {
                DijkstraStep dijkstraNext = subGraph.Find(n => n.pathStep == next);
                if (dijkstraNext.weight > current.weight + PathStep.Distance(current.pathStep, dijkstraNext.pathStep))
                {
                    dijkstraNext.weight = current.weight + PathStep.Distance(current.pathStep, dijkstraNext.pathStep);
                    dijkstraNext.prev = current;
                }
            }

            unprocessed.Remove(current);
        }

        return subGraph;
    }

    public List<Vector2Int> FindShorterPathFromTo (Vector2Int from, Vector2Int to, List<PathStep> graph)
    {
        // Find from and end postitions in graph
        PathStep start = null, end = null;
        if (graph != null)
        {
            start = graph.Find(step => step.pos == from);
            end = graph.Find(step => step.pos == to);
        }

        if (graph == null || start == null || end == null)
        {
            Debug.Log("No path!");
            return null;
        }

        // Output initialization
        List<Vector2Int> path = new List<Vector2Int>();

        // Build Subgraph
        List<DijkstraStep> subGraph = GetSubGraph(from, graph);

        // Retrieves shorter path step by step, from end to start
        DijkstraStep shortestPathStep = subGraph.Find(s => s.pathStep == end);
        
        while (shortestPathStep.pathStep != start)
        {
            path.Add(shortestPathStep.pathStep.pos);
            shortestPathStep = shortestPathStep.prev;
        }
        path.Add(start.pos);

        // Revert path so it's from start to end
        path.Reverse();

        return path;
    }
}
