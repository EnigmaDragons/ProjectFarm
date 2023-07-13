using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class LevelMapSpawner : OnMessage<LevelResetApproved>
{
    [Header("GenConfig")]
    [SerializeField] private bool generateOnAwake = true;
    [SerializeField] private LevelGenV1Params genParams;
    
    [Header("State")]
    [SerializeField] private GameState game;
    [SerializeField] private CurrentLevel currentLevel;
    [SerializeField] private GameObject parent;
    [SerializeField] private GameObject settingParent;

    [Header("Pieces")]
    [SerializeField] private GameObject protoBarn;
    [SerializeField] private GameObject protoHero;
    [SerializeField] private GameObject protoFood;
    [FormerlySerializedAs("protoStarFood")] [SerializeField] private GameObject protoTreat;
    [SerializeField] private GameObject protoFloor;
    [SerializeField] private GameObject protoWater;
    [SerializeField] private GameObject protoDolphin;
    [SerializeField] private GameObject protoDolphinRideExit;

    [Header("Setting")] 
    [SerializeField] private GameObject protoEmpty;
    [SerializeField] private Vector2Int settingPadding;

    private Dictionary<MapPiece, GameObject> _mapPiecePrototypes;

    void Awake()
    {
        _mapPiecePrototypes = new Dictionary<MapPiece, GameObject>
        {
            { MapPiece.HeroAnimal, protoHero },
            { MapPiece.Floor, protoFloor },
            { MapPiece.Barn, protoBarn },
            { MapPiece.Food, protoFood },
            { MapPiece.Treat, protoTreat },
            { MapPiece.Water, protoWater },
            { MapPiece.Dolphin, protoDolphin },
            { MapPiece.DolphinRideExit, protoDolphinRideExit },
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
        try
        {
            Instantiate(GenPipeline.CreateOne(genParams));
        }
        catch (Exception e)
        {
            Log.Error(e);
        }
    }

    protected override void Execute(LevelResetApproved msg)
    {
        if (currentLevel.ActiveMap == null)
            return;

        Instantiate(currentLevel.ActiveMap);
    }

    private void Instantiate(GameObject proto, Vector3 pos)
    {
        Instantiate(proto, pos, Quaternion.identity, parent.transform);
    }
    
    private void Instantiate(LevelMap level)
    {
        foreach (Transform child in parent.transform)
        {
            var g = child.gameObject;
            g.SetActive(false);
            Destroy(g);
        }
        foreach (Transform child in settingParent.transform)
        {
            var g = child.gameObject;
            g.SetActive(false);
            Destroy(g);
        }
        
        currentLevel.UseGenMap(level, parent.transform);
        game.BeginInitGeneratedLevelMap();
        var tilesGenerated = new HashSet<TilePoint>();
        foreach (var (x, y) in level.GetIterator())
        {
            var location = new Vector3(x, 0, y);
            tilesGenerated.Add(new TilePoint(location));
            var floor = level.FloorLayer[x, y];
            if (_mapPiecePrototypes.TryGetValue(floor, out var proto))
                Instantiate(proto, location);
            else
                Instantiate(protoEmpty, location);
            
            var piece = level.ObjectLayer[x, y];
            if (_mapPiecePrototypes.TryGetValue(piece, out var proto2))
                Instantiate(proto2, location);
        }
        for (var x = 0 - settingPadding.x; x < level.Width + settingPadding.x; x++)
            for (var y = 0 - settingPadding.y; y < level.Height + settingPadding.y; y++)
            {
                var location = new Vector3(x, 0, y);
                if (!tilesGenerated.Contains(new TilePoint(location)))
                    Instantiate(protoEmpty, location, Quaternion.identity, settingParent.transform);
            }
        
        Log.SInfo(LogScopes.GameFlow, $"Instantiated Generated Map");
        game.FinishInitGeneratedLevelMap();
    }
}
