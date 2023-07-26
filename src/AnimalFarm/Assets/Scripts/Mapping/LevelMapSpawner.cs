using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelMapSpawner : OnMessage<LevelResetApproved, LevelRegenRequested, SpawnMapPieceRequested>
{
    [Header("GenConfig")]
    [SerializeField] private bool generateOnAwake = true;
    [SerializeField] private LevelGenV1Params genParams;
    
    [Header("State")]
    [SerializeField] private GameState game;
    [SerializeField] private CurrentLevel currentLevel;
    [SerializeField] private CurrentLevelMap currentMap;
    [SerializeField] private GameObject parent;
    [SerializeField] private GameObject settingParent;

    [Header("Floors")] 
    [SerializeField] private GameObject protoNothing;
    [SerializeField] private GameObject protoDirt;
    [SerializeField] private GameObject protoSeedling;
    [SerializeField] private GameObject protoRiver;
    [SerializeField] private GameObject protoFissure;
    
    [Header("Pieces")]
    [SerializeField] private GameObject protoBarn;
    [SerializeField] private GameObject protoHero;
    [SerializeField] private GameObject protoFood;
    [SerializeField] private GameObject protoTreat;
    [SerializeField] private GameObject protoDolphin;
    [SerializeField] private GameObject protoDolphinRideExit;
    [SerializeField] private GameObject protoElephant;
    [SerializeField] private GameObject protoDino;

    [Header("Setting")] 
    [SerializeField] private GameObject protoEmpty;
    [SerializeField] private Vector2Int settingPadding;

    private Dictionary<MapPiece, GameObject> _mapPiecePrototypes;
    
    void Awake()
    {
        _mapPiecePrototypes = new Dictionary<MapPiece, GameObject>
        {
            { MapPiece.Nothing, protoNothing },
            { MapPiece.Dirt, protoDirt },
            { MapPiece.River, protoRiver },
            { MapPiece.Seedling, protoSeedling },
            { MapPiece.Fissure, protoFissure },
            { MapPiece.HeroAnimal, protoHero },
            { MapPiece.Barn, protoBarn },
            { MapPiece.Food, protoFood },
            { MapPiece.Treat, protoTreat },
            { MapPiece.Dolphin, protoDolphin },
            { MapPiece.DolphinRideExit, protoDolphinRideExit },
            { MapPiece.Elephant, protoElephant },
            { MapPiece.Dino, protoDino },
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
            Instantiate(GenPipeline.CreateOne(genParams), isReset: false);
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

        Instantiate(currentLevel.ActiveMap, isReset: true);
    }

    protected override void Execute(LevelRegenRequested msg)
    {
        Generate();
    }

    protected override void Execute(SpawnMapPieceRequested msg)
    {
        Instantiate(_mapPiecePrototypes[msg.Piece], new Vector3(msg.Tile.X, 0, msg.Tile.Y));
    }

    private GameObject Instantiate(GameObject proto, Vector3 pos)
    {
        return Instantiate(proto, pos, Quaternion.identity, parent.transform);
    }
    
    private void Instantiate(LevelMap level, bool isReset)
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
        game.BeginInitGeneratedLevelMap(isReset);
        var tilesGenerated = new HashSet<TilePoint>();
        var fissure = new HashSet<TilePoint>();
        foreach (var (x, y) in level.GetIterator())
        {
            var location = new Vector3(x, 0, y);
            tilesGenerated.Add(new TilePoint(location));
            var floor = level.FloorLayer[x, y];
            if (floor == MapPiece.Fissure)
                fissure.Add(new TilePoint(x, y));
            if (_mapPiecePrototypes.TryGetValue(floor, out var proto))
                Instantiate(proto, location);

            var piece = level.ObjectLayer[x, y];
            if (piece != MapPiece.Nothing && _mapPiecePrototypes.TryGetValue(piece, out var proto2))
                Instantiate(proto2, location);
        }
        
        var finalPadding = fissure.Count > 0
            ? settingPadding + Vector2Int.one
            : settingPadding;
        var settingPieces = new List<GameObject>();
        for (var x = -finalPadding.x; x < level.Width + finalPadding.x; x++)
            for (var y = -finalPadding.y; y < level.Height + finalPadding.y; y++)
            {
                var location = new Vector3(x, 0, y);
                if (!tilesGenerated.Contains(new TilePoint(location)))
                    settingPieces.Add(Instantiate(protoEmpty, location, Quaternion.identity, settingParent.transform));
            }
        currentMap.RegisterSetting(settingPieces.ToArray());
        
        Log.SInfo(LogScopes.GameFlow, $"Instantiated Generated Map");
        game.FinishInitGeneratedLevelMap(isReset);
    }
}
