using System;
using UnityEngine;
using System.Collections.Generic;
using Priority_Queue;

public class GridGraph
{
    public int Width;
    public int Height;

    public AStarMutableNode[,] Grid;
    public HashSet<Vector2Int> Impassables = new HashSet<Vector2Int>();

    public GridGraph(int w, int h)
    {
        Width = w;
        Height = h;

        Grid = new AStarMutableNode[w, h];

        for (var x = 0; x < w; x++)
            for (var y = 0; y < h; y++)
                Grid[x, y] = new AStarMutableNode(x, y);
    }
    
    public bool InBounds(Vector2Int v) =>
        v.x >= 0 && v.x < Width &&
        v.y >= 0 && v.y < Height;
    
    public bool Passable(Vector2Int v) => !Impassables.Contains(v);

    private static readonly List<Vector2Int> CardinalOffsets = new List<Vector2Int>()
    {
        new Vector2Int(-1, 0), // left
        new Vector2Int(0, 1),  // top
        new Vector2Int(1, 0),  // right
        new Vector2Int(0, -1), // bottom
    };
    
    public List<AStarMutableNode> CardinalNeighbours(AStarMutableNode n)
    {
        var results = new List<AStarMutableNode>();
        
        foreach (var v in CardinalOffsets)
        {
            var newVector = v + n.Position;
            if (InBounds(newVector) && Passable(newVector)) 
                results.Add(Grid[newVector.x, newVector.y]);
        }

        return results;
    }
    
    private static readonly List<Vector2Int> OrthogonalOffsets = new List<Vector2Int>()
    {
        new Vector2Int(-1, 0), // left
        new Vector2Int(-1, 1),  // top-left
        new Vector2Int(0, 1),  // top
        new Vector2Int(1, 1),  // top-right
        new Vector2Int(1, 0),  // right
        new Vector2Int(1, -1), // bottom-right
        new Vector2Int(0, -1), // bottom
        new Vector2Int(-1, -1) // bottom-left
    };
    
    public List<AStarMutableNode> OrthogonalNeighbours(AStarMutableNode n)
    {
        var results = new List<AStarMutableNode>();
        foreach (var v in OrthogonalOffsets)
        {
            var newVector = v + n.Position;
            if (InBounds(newVector) && Passable(newVector))
                results.Add(Grid[newVector.x, newVector.y]);
        }
        return results;
    }

    public int Cost(AStarMutableNode b) => 1;
}


public class AStarMutableNode : IComparable<AStarMutableNode>
{
    public Vector2Int Position { get; }
    public float Priority { get; set; }

    public AStarMutableNode(int x, int y) => Position = new Vector2Int(x, y);

    public int CompareTo(AStarMutableNode other)
    {
        if (Priority < other.Priority) return -1;
        if (Priority > other.Priority) return 1;
        return 0;
    }
}

public static class AStar
{
    public static List<AStarMutableNode> Search(GridGraph graph, AStarMutableNode start, AStarMutableNode goal)
    {
        var cameFrom = new Dictionary<AStarMutableNode, AStarMutableNode>();
        var costSoFar = new Dictionary<AStarMutableNode, float>();

        var path = new List<AStarMutableNode>();

        var frontier = new SimplePriorityQueue<AStarMutableNode>();
        frontier.Enqueue(start, 0);

        cameFrom.Add(start, start);
        costSoFar.Add(start, 0);

        var current = new AStarMutableNode(0,0);
        while (frontier.Count > 0)
        {
            current = frontier.Dequeue();
            if (current == goal) break; // Early exit

            foreach (var next in graph.CardinalNeighbours(current))
            {
                var newCost = costSoFar[current] + graph.Cost(next);
                if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                {
                    costSoFar[next] = newCost;
                    cameFrom[next] = current;
                    var priority = newCost + Heuristic(next, goal);
                    frontier.Enqueue(next, priority);
                    next.Priority = newCost;
                }
            }
        }

        while (current != start)
        {
            path.Add(current);
            current = cameFrom[current];
        }
        path.Reverse();

        return path;
    }

    private static float Heuristic(AStarMutableNode a, AStarMutableNode b) => Mathf.Abs(a.Position.x - b.Position.x) + Mathf.Abs(a.Position.y - b.Position.y);
}