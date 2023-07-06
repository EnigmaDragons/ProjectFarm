using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelMapSpawner : MonoBehaviour
{
    [Header("GenConfig")]
    [SerializeField] private bool generateOnAwake = true;
    [SerializeField] private int numMinMoves = 12;
    
    [Header("State")]
    [SerializeField] private GameState game;
    [SerializeField] private CurrentLevel currentLevel;
    [SerializeField] private GameObject parent;

    [Header("Pieces")]
    [SerializeField] private GameObject protoBarn;
    [SerializeField] private GameObject protoHero;
    [SerializeField] private GameObject protoFood;
    [SerializeField] private GameObject protoStarFood;
    [SerializeField] private GameObject protoFloor;
    
    [Header("Victory Conditions")]
    [SerializeField] private VictoryCondition[] victoryConditions;

    private Dictionary<MapPiece, GameObject> _mapPiecePrototypes;

    void Awake()
    {
        _mapPiecePrototypes = new Dictionary<MapPiece, GameObject>
        {
            { MapPiece.HeroAnimal, protoHero },
            { MapPiece.Floor, protoFloor },
            { MapPiece.Barn, protoBarn },
            { MapPiece.Food, protoFood },
            { MapPiece.StarFood, protoStarFood }
        };
        if (generateOnAwake)
            Generate();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Generate();
    }
    
    private void Generate()
    {
        foreach (Transform child in parent.transform) {
            Destroy(child.gameObject);
        }

        var level = GenPipeline.CreateOne();
        currentLevel.UseGenMap(level, parent.transform);
        game.BeginInitGeneratedLevelMap();
        foreach (var (x, y) in level.GetIterator())
        {
            var floor = level.FloorLayer[x, y];
            if (_mapPiecePrototypes.TryGetValue(floor, out var proto))
                Instantiate(proto, new Vector3(x, 0, y), Quaternion.identity, parent.transform);
        }
        foreach (var (x, y) in level.GetIterator())
        {
            var piece = level.ObjectLayer[x, y];
            if (_mapPiecePrototypes.TryGetValue(piece, out var proto2))
                Instantiate(proto2, new Vector3(x, 0, y), Quaternion.identity, parent.transform);
        }
        game.FinishInitGeneratedLevelMap();
    }
    
}

