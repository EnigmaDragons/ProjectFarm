using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class LevelMapSpawner : MonoBehaviour
{
    [Header("GenConfig")]
    [SerializeField] private bool generateOnAwake = true;
    [SerializeField] private LevelGenV1Params genParams;
    
    [Header("State")]
    [SerializeField] private GameState game;
    [SerializeField] private CurrentLevel currentLevel;
    [SerializeField] private GameObject parent;

    [Header("Pieces")]
    [SerializeField] private GameObject protoBarn;
    [SerializeField] private GameObject protoHero;
    [SerializeField] private GameObject protoFood;
    [FormerlySerializedAs("protoStarFood")] [SerializeField] private GameObject protoTreat;
    [SerializeField] private GameObject protoFloor;
    
    private Dictionary<MapPiece, GameObject> _mapPiecePrototypes;

    void Awake()
    {
        _mapPiecePrototypes = new Dictionary<MapPiece, GameObject>
        {
            { MapPiece.HeroAnimal, protoHero },
            { MapPiece.Floor, protoFloor },
            { MapPiece.Barn, protoBarn },
            { MapPiece.Food, protoFood },
            { MapPiece.Treat, protoTreat }
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

        var level = GenPipeline.CreateOne(genParams);
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

