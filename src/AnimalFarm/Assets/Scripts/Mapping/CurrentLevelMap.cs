using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class CurrentLevelMap : ScriptableObject
{
    [SerializeField] private Transform finalCameraAngle;
    [SerializeField] private Vector2 min;
    [SerializeField] private Vector2 max;
    [SerializeField] private string levelName;
    
    [SerializeField] private List<MovementOptionRule> movementOptionRules = new List<MovementOptionRule>();
    [SerializeField] private List<MovementRestrictionRule> movementRestrictionRules = new List<MovementRestrictionRule>();

    private Dictionary<GameObject, MapPieceWithRules> _pieces = new Dictionary<GameObject, MapPieceWithRules>();
    private Dictionary<GameObject, MapPieceWithRules> _destroyedObjects = new Dictionary<GameObject, MapPieceWithRules>();

    public Vector2 Min => min;
    public GameObject Hero => _pieces.Single(p => p.Value.Piece == MapPiece.HeroAnimal).Key;
    public TilePoint BarnLocation => new TilePoint(_pieces.Single(p => p.Value.Piece == MapPiece.Barn).Key);
    public int NumSelectableObjects => _pieces.Count(p => p.Value.Rules.IsSelectable);
    public IEnumerable<MovementOptionRule> MovementOptionRules => movementOptionRules;
    public IEnumerable<MovementRestrictionRule> MovementRestrictionRules => movementRestrictionRules;
    public IEnumerable<GameObject> Selectables => _pieces.Where(p => p.Value.Rules.IsSelectable).Select(x => x.Key);
    public int NumOfJumpables => _pieces.Count(p => p.Value.Rules.IsJumpable);
    public Transform FinalCameraAngle => finalCameraAngle;
    public IEnumerable<GameObject> Walkables => _pieces.Where(p => p.Value.Rules.IsWalkable).Select(x => x.Key);
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
        _pieces = new Dictionary<GameObject, MapPieceWithRules>();
        _destroyedObjects = new Dictionary<GameObject, MapPieceWithRules>();
        movementOptionRules = new List<MovementOptionRule>();
        movementRestrictionRules = new List<MovementRestrictionRule>();
    }

    public void AddMovementOptionRule(MovementOptionRule optionRule) => movementOptionRules.Add(optionRule);
    public void AddMovementRestrictionRule(MovementRestrictionRule restrictionRule) => movementRestrictionRules.Add(restrictionRule);

    public void Register(GameObject obj, MapPiece piece) => _pieces[obj] = new MapPieceWithRules { Piece = piece, Rules = piece.Rules() };

    public void RegisterFinalCameraAngle(Transform t) => finalCameraAngle = t;
    
    public Maybe<GameObject> GetTile(TilePoint tile) => _pieces
        .Where(x => x.Value.Rules.IsFloor)
        .Select(x => x.Key)
        .FirstAsMaybe(o => new TilePoint(o).Equals(tile));
    public Maybe<GameObject> GetSelectable(TilePoint tile) =>  Selectables.FirstAsMaybe(o => new TilePoint(o).Equals(tile));
    
    public bool IsJumpable(TilePoint tile) =>
        _pieces.Any(w => new TilePoint(w.Key).Equals(tile) && w.Value.Rules.IsJumpable);
    public bool IsWalkable(TilePoint tile) =>
        _pieces.Any(w => new TilePoint(w.Key).Equals(tile) && w.Value.Rules.IsWalkable);
    public bool IsBlocked(TilePoint tile) => 
        _pieces.Any(w => new TilePoint(w.Key).Equals(tile) && w.Value.Rules.IsBlocking);
    public bool IsEdible(TilePoint tile) => 
        _pieces.Any(w => new TilePoint(w.Key).Equals(tile) && w.Value.Rules.IsEdible);
    public bool IsEnterable(TilePoint tile) => 
        _pieces.Any(w => new TilePoint(w.Key).Equals(tile) && w.Value.Rules.IsEnterable);
    
    public void Move(GameObject obj, TilePoint from, TilePoint to)
        => Notify(() => {});

    public void RestoreDestroyed(GameObject obj)
    {
        Notify(() =>
        {
            if (_destroyedObjects.TryGetValue(obj, out var pieceWithRules))
                Register(obj, pieceWithRules.Piece);
        });
    }
    
    public void Remove(GameObject obj)
    {
        Notify(() =>
        {
            _destroyedObjects[obj] = _pieces[obj];
            _pieces.Remove(obj);
        });
    }

    public MapPiece GetPiece(GameObject obj) => _pieces.TryGetValue(obj, out var pieceWithRules)
        ? pieceWithRules.Piece
        : MapPiece.Nothing;
    
    public LevelMap GetLevelMap()
    {
        var builder = new LevelMapBuilder(levelName, Mathf.CeilToInt(max.x + 1), Mathf.CeilToInt(max.y + 1));
        _pieces.ForEach(p => builder.With(new TilePoint(p.Key), p.Value.Piece));
        return builder.Build();
    }

    private LevelStateSnapshot GetSnapshot()
    {
        // TODO: Cache on change, instead of generating on query
        var maxX = 0;
        var maxY = 0;
        var floors = new Dictionary<TilePoint, MapPiece>();
        var pieces = new Dictionary<TilePoint, MapPiece>();
        // TODO: Track real counters
        var counters = new Dictionary<CounterType, int>();

        foreach (var p in _pieces)
        {
            var tp = new TilePoint(p.Key);
            if (p.Value.Rules.IsFloor)
                floors[tp] = p.Value.Piece;
            else
                pieces[tp] = p.Value.Piece;
            maxX = Math.Max(maxX, tp.X);
            maxY = Math.Max(maxY, tp.Y);
        }

        return new LevelStateSnapshot(new Vector2Int(maxX, maxY), floors, pieces, counters);
    }

    private LevelStateSnapshot _snapshot;
    public LevelStateSnapshot Snapshot
    {
        get
        {
            if (_snapshot == null)
                _snapshot = GetSnapshot();
            return _snapshot;
        }
    }
    
    private void Notify(Action a)
    {
        var before = Snapshot;
        a();
        _snapshot = GetSnapshot();
        Message.Publish(new LevelStateChanged { Before = before, After = _snapshot });
    }
}
