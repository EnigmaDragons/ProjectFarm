using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class CurrentLevelMap : ScriptableObject
{
    [SerializeField] private TilePoint bitVaultLocation;
    [SerializeField] private GameObject hero;
    [SerializeField] private List<GameObject> walkableTiles = new List<GameObject>();
    [SerializeField] private List<GameObject> blockedTiles = new List<GameObject>();
    [SerializeField] private HashSet<GameObject> jumpableObjects = new HashSet<GameObject>();
    [SerializeField] private List<GameObject> selectableObjects = new List<GameObject>();
    [SerializeField] private List<GameObject> edibleObjects = new List<GameObject>();
    [SerializeField] private List<GameObject> collectibleObjects = new List<GameObject>();
    [SerializeField] private List<GameObject> enterableObjects = new List<GameObject>();
    [SerializeField] private Dictionary<GameObject, ObjectRules> destroyedObjects = new Dictionary<GameObject, ObjectRules>();
    [SerializeField] private Transform finalCameraAngle;
    [SerializeField] private Vector2 min;
    [SerializeField] private Vector2 max;
    [SerializeField] private string levelName;
    
    [SerializeField] private List<MovementOptionRule> movementOptionRules = new List<MovementOptionRule>();
    [SerializeField] private List<MovementRestrictionRule> movementRestrictionRules = new List<MovementRestrictionRule>();

    public Vector2 Min => min;
    public Vector2 Max => max;
    public GameObject Hero => hero;
    public TilePoint BitVaultLocation => bitVaultLocation;
    public int NumSelectableObjects => selectableObjects.Count;
    public IEnumerable<MovementOptionRule> MovementOptionRules => movementOptionRules;
    public IEnumerable<MovementRestrictionRule> MovementRestrictionRules => movementRestrictionRules;
    public IEnumerable<GameObject> Selectables => selectableObjects;
    public int NumOfJumpables => jumpableObjects.Count;
    public Transform FinalCameraAngle => finalCameraAngle;
    public IEnumerable<GameObject> Walkables => walkableTiles;
    public IEnumerable<MovementOptionRule> MovementOptions => movementOptionRules;
    public IEnumerable<MovementRestrictionRule> MovementRestrictions => movementRestrictionRules;

    public bool HasLost { get; set; }

    public void InitLevel(string activeLevelName)
    {
        Debug.Log($"Init Level");
        HasLost = false;
        levelName = activeLevelName;
        min = new Vector2();
        max = new Vector2();
        bitVaultLocation = null;
        hero = null;
        walkableTiles = new List<GameObject>();
        blockedTiles = new List<GameObject>();
        jumpableObjects = new HashSet<GameObject>();
        selectableObjects = new List<GameObject>();
        collectibleObjects = new List<GameObject>();
        edibleObjects = new List<GameObject>();
        enterableObjects = new List<GameObject>();
        destroyedObjects = new Dictionary<GameObject, ObjectRules>();
        movementOptionRules = new List<MovementOptionRule>();
        movementRestrictionRules = new List<MovementRestrictionRule>();
    }

    public void RegisterHero(GameObject obj) => hero = obj;
    public void AddMovementOptionRule(MovementOptionRule optionRule) => movementOptionRules.Add(optionRule);
    public void AddMovementRestrictionRule(MovementRestrictionRule restrictionRule) => movementRestrictionRules.Add(restrictionRule);

    public void RegisterAsSelectable(GameObject obj) => selectableObjects.Add(obj);
    public void RegisterAsJumpable(GameObject obj)
    {
        Debug.Log($"Jumpable {obj.name}");
        jumpableObjects.Add(obj);
    }

    public void RegisterAsEdible(GameObject obj) => edibleObjects.Add(obj);

    public void RegisterBitVault(GameObject obj) => bitVaultLocation = new TilePoint(obj);
    public void RegisterWalkableTile(GameObject obj) => UpdateSize(() => walkableTiles.Add(obj));
    public void RegisterBlockingObject(GameObject obj) => UpdateSize(() => blockedTiles.Add(obj));
    public void RegisterAsCollectible(GameObject obj) => collectibleObjects.Add(obj);
    public void RegisterAsEnterable(GameObject obj) => enterableObjects.Add(obj);
    public void RegisterFinalCameraAngle(Transform t) => finalCameraAngle = t;

    private void UpdateSize(Action a)
    {
        a();
        if (walkableTiles == null || walkableTiles.Any(w => w == null))
            return;
        
        var tiles = walkableTiles.Concat(blockedTiles).Select(x => new TilePoint(x)).ConcatIfNotNull(bitVaultLocation).ToList();
        min = new Vector2(tiles.Min(t => t.X), tiles.Min(t => t.Y));
        max = new Vector2(tiles.Max(t => t.X), tiles.Max(t => t.Y));
    }

    public Maybe<GameObject> GetTile(TilePoint tile) => walkableTiles.FirstAsMaybe(o => new TilePoint(o).Equals(tile));
    public Maybe<GameObject> GetSelectable(TilePoint tile) =>  selectableObjects.FirstAsMaybe(o => new TilePoint(o).Equals(tile));

    public bool IsJumpable(TilePoint tile) => jumpableObjects.Any(t => new TilePoint(t).Equals(tile));
    public bool IsWalkable(TilePoint tile) => walkableTiles.Any(w => new TilePoint(w).Equals(tile));
    public bool IsBlocked(TilePoint tile) => blockedTiles.Any(t => new TilePoint(t).Equals(tile));
    public bool IsEdible(TilePoint tile) => edibleObjects.Any(t => new TilePoint(t).Equals(tile));
    public bool IsEnterable(TilePoint tile) => enterableObjects.Any(t => new TilePoint(t).Equals(tile));
    
    public void Move(GameObject obj, TilePoint from, TilePoint to)
        => Notify(() => {});

    public void RestoreDestroyed(GameObject obj)
    {
        Notify(() =>
        {
            if (!destroyedObjects.TryGetValue(obj, out var rules)) return;
            
            if (rules.IsJumpable)
                RegisterAsJumpable(obj);
            if (rules.IsBlocking)
                RegisterBlockingObject(obj);
            if (rules.IsSelectable)
                RegisterAsSelectable(obj);
            if (rules.IsWalkable)
                RegisterWalkableTile(obj);
            if (rules.IsCollectible)
                RegisterAsCollectible(obj);
            if (rules.IsEdible)
                RegisterAsEdible(obj);
            if (rules.IsEnterable)
                RegisterAsEnterable(obj);
        });
    }
    
    public void Remove(GameObject obj)
    {
        Notify(() =>
        {
            destroyedObjects[obj] = new ObjectRules
            {
                IsWalkable = walkableTiles.Remove(obj),
                IsJumpable = jumpableObjects.Remove(obj),
                IsBlocking = blockedTiles.Remove(obj),
                IsSelectable = selectableObjects.Remove(obj),
                IsCollectible = collectibleObjects.Remove(obj),
                IsEdible = edibleObjects.Remove(obj),
                IsEnterable = enterableObjects.Remove(obj)
            };
        });
    }

    public LevelMap GetLevelMap()
    {
        var builder = new LevelMapBuilder(levelName, Mathf.CeilToInt(max.x + 1), Mathf.CeilToInt(max.y + 1));
        walkableTiles.ForEach(x =>
        {
            var t = new TilePoint(x);
            var fallingTile = x.GetComponent<FallingTile>();
            if (fallingTile)
                builder.With(t, MapPiece.FailsafeFloor);
            else
                builder.With(t, MapPiece.Floor);
        });
        
        jumpableObjects.ForEach(x =>
        {
            var t = new TilePoint(x);
            if (x.GetComponent<DestroyIfDoubleJumped>())
                builder.With(t, MapPiece.DoubleRoutine);
            else if (x.GetComponent<TeleportingPiece>())
                builder.With(t, MapPiece.JumpingRoutine);
            else if (x.GetComponent<DestroyIfJumped>())
                builder.With(t, MapPiece.Routine);
        });

        collectibleObjects.ForEach(x => builder.With(new TilePoint(x), MapPiece.DataCube));
        
        builder.With(bitVaultLocation, MapPiece.Root);
        builder.With(new TilePoint(hero), MapPiece.RootKey);

        return builder.Build();
    }
    
    //This code is a bit concrete
    public LevelSimulationSnapshot GetSnapshot()
    {
        var floors = new List<TilePoint>();
        var disengagedFailsafes = new List<TilePoint>();
        walkableTiles.ForEach(x =>
        {
            var fallingTile = x.GetComponent<FallingTile>();
            if (fallingTile && !fallingTile.IsDangerous)
                disengagedFailsafes.Add(new TilePoint(x));
            else if (!fallingTile)
                floors.Add(new TilePoint(x));
        });
        var oneHealthSubroutines = new List<TilePoint>();
        var twoHealthSubroutines = new List<TilePoint>();
        var iceSubroutines = new List<TilePoint>();
        jumpableObjects.ForEach(x =>
        {
            var doubleHealth = x.GetComponent<DestroyIfDoubleJumped>();
            if (doubleHealth && doubleHealth.NumJumpsRemaining == 2)
                twoHealthSubroutines.Add(new TilePoint(x));
            else if (doubleHealth && doubleHealth.NumJumpsRemaining == 1)
                oneHealthSubroutines.Add(new TilePoint(x));
            else if (x.GetComponent<TeleportingPiece>())
                iceSubroutines.Add(new TilePoint(x));
            else if (x.GetComponent<DestroyIfJumped>())
                oneHealthSubroutines.Add(new TilePoint(x));
        });
        return new LevelSimulationSnapshot(floors, disengagedFailsafes, oneHealthSubroutines, twoHealthSubroutines, iceSubroutines, collectibleObjects.Select(x => new TilePoint(x)).ToList(), new TilePoint(hero), bitVaultLocation);
    }

    public int CountDangerousTiles()
        => walkableTiles.Count(x =>
        {
            var fallingTile = x.GetComponent<FallingTile>();
            return fallingTile != null && fallingTile.IsDangerous;
        });

    private void Notify(Action a)
    {
        a();
        Message.Publish(new LevelStateChanged());
    }
}
