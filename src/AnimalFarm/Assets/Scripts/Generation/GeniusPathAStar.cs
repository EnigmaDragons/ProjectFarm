using System.Linq;
using UnityEngine;

public class GeniusPathAStar
{
    public static TilePoint[] GetBestPath(LevelMap m, bool allowMovementOnNothingFloor = true)
    {
        var gridGraph = new GridGraph(m.Width, m.Height);
        var heroLoc = new Vector2Int(-1, -1);
        var barnLoc = new Vector2Int(-1, -1);
        m.GetIterator().ForEach((xy) =>
        {
            var x = xy.Item1;
            var y = xy.Item2;
            var v = new Vector2Int(x, y);
            var floor = m.FloorLayer[x, y];
            if (!floor.Rules().IsWalkable)
                gridGraph.Impassables.Add(v);
            if (!allowMovementOnNothingFloor && floor == MapPiece.Nothing)
                gridGraph.Impassables.Add(v);

            var obj = m.ObjectLayer[x, y];
            if (obj.Rules().IsBlocking)
                gridGraph.Impassables.Add(v);
            if (obj == MapPiece.HeroAnimal)
                heroLoc = v;
            if (obj == MapPiece.Barn)
                barnLoc = v;
        });
        
        
        var path = AStar.Search(gridGraph, gridGraph.Grid[heroLoc.x, heroLoc.y], gridGraph.Grid[barnLoc.x, barnLoc.y]);
        return path.Select(p => new TilePoint(p.Position.x, p.Position.y)).ToArray();
    }
}