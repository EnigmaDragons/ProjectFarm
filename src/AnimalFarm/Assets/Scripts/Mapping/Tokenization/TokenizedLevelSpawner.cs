using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public sealed class TokenizedLevelSpawner : ScriptableObject
{
    [SerializeField] private GameObject floor;
    [SerializeField] private GameObject root;
    [SerializeField] private GameObject rootKey;
    [SerializeField] private GameObject dataCube;
    [SerializeField] private GameObject subroutine;

    public GameObject Spawn(string level)
    {
        var pieces = new Dictionary<MapPiece, GameObject>
        {
            { MapPiece.Floor, floor },
            { MapPiece.Barn, root },
            { MapPiece.HeroAnimal, rootKey },
            { MapPiece.StarFood, dataCube },
            { MapPiece.Food, subroutine },
        };
        
        var map = TokenizedLevelMap.FromString(level);
        var iterator = new TwoDimensionalIterator(map.Width, map.Height);
        var obj = new GameObject();
        iterator.Select(p => new TilePoint(p.Item1, p.Item2)).ForEach(t =>
        {
            Instantiate(pieces[map.FloorLayer[t.X, t.Y]], new Vector3(t.X, t.Y, 0), Quaternion.identity, obj.transform);
            Instantiate(pieces[map.ObjectLayer[t.X, t.Y]], new Vector3(t.X, t.Y, 0), Quaternion.identity, obj.transform);
        });

        return obj;
    }

}
