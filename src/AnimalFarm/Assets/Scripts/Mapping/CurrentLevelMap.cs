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

    private GameObject _hero;
    private GameObject[] _settingPieces = Array.Empty<GameObject>();
    private Dictionary<GameObject, MapPieceWithRules> _pieces = new Dictionary<GameObject, MapPieceWithRules>();
    private Dictionary<GameObject, MapPieceWithRules> _destroyedObjects = new Dictionary<GameObject, MapPieceWithRules>();
    private Vector2Int[] _heroPath = Array.Empty<Vector2Int>();
    private LevelStateSnapshot _snapshot;
    private readonly Dictionary<CounterType, int> _counters = new Dictionary<CounterType, int>();
    
    public int this[CounterType type] => _counters.ValueOrDefault(type, () => 0);

    public Vector2 Min => min;
    public GameObject Hero => _hero;
    public TilePoint InitialHeroLocation => _heroPath != null && _heroPath.Length > 0 ? new TilePoint(_heroPath[0].x, _heroPath[0].y) : new TilePoint(-1, -1);
    public Vector2Int[] HeroPath => _heroPath ?? Array.Empty<Vector2Int>();
    public HeroAnimal GeniusAnimal => _heroPath != null && _heroPath.Length > 2 ? (HeroAnimal)(_heroPath.Length - 2) : HeroAnimal.NotSelected;
    public TilePoint BarnLocation => new TilePoint(_pieces.Single(p => p.Value.Piece == MapPiece.Barn).Key);
    public int NumSelectableObjects => _pieces.Count(p => p.Value.Rules.IsSelectable);
    public IEnumerable<GameObject> Selectables => _pieces.Where(p => p.Value.Rules.IsSelectable).Select(x => x.Key);
    public int NumOfJumpables => _pieces.Count(p => p.Value.Rules.IsJumpable);
    public Transform FinalCameraAngle => finalCameraAngle;

    public bool HasLost { get; set; }

    public void InitLevel(string activeLevelName)
    {
        Log.SInfo(LogScopes.GameFlow, "Init Current Level Map");
        HasLost = false;
        levelName = activeLevelName;
        min = new Vector2();
        max = new Vector2();
        _settingPieces = Array.Empty<GameObject>();
        _pieces = new Dictionary<GameObject, MapPieceWithRules>();
        _destroyedObjects = new Dictionary<GameObject, MapPieceWithRules>();
        movementOptionRules = new List<MovementOptionRule>();
        movementRestrictionRules = new List<MovementRestrictionRule>();
        _counters.Clear();
        _snapshot = null;
        _hero = null;
        _heroPath = Array.Empty<Vector2Int>();
    }

    public void FinalizeInitialCounters()
    {
        _counters[CounterType.NumTreatsPossible] = _pieces.Count(p => p.Value.Piece == MapPiece.Treat);
        _counters[CounterType.NumFoodPossible] = _pieces.Count(p => p.Value.Piece == MapPiece.Food || p.Value.Piece == MapPiece.Seedling);
        _counters[CounterType.NumMovesMade] = 0;
        Refresh();
    }

    [Obsolete]
    public void AddMovementOptionRule(MovementOptionRule optionRule) => movementOptionRules.Add(optionRule);
    [Obsolete]
    public void AddMovementRestrictionRule(MovementRestrictionRule restrictionRule) => movementRestrictionRules.Add(restrictionRule);

    public void RegisterHeroPath(Vector2Int[] path)
    {
        _heroPath = path;
    }
    
    public void Register(GameObject obj, MapPiece piece)
    {
        _pieces[obj] = new MapPieceWithRules { Piece = piece, Rules = piece.Rules() };
        if (piece == MapPiece.HeroAnimal)
            _hero = obj;
    }

    public void RegisterSetting(GameObject[] objs) => _settingPieces = objs;

    public void RegisterFinalCameraAngle(Transform t) => finalCameraAngle = t;

    public IEnumerable<GameObject> SettingPieces => _settingPieces;
    public IEnumerable<KeyValuePair<GameObject, MapPieceWithRules>> Pieces => _pieces;
    public Maybe<GameObject> GetFloorTile(TilePoint tile) => _pieces
        .Where(x => x.Value.Rules.IsFloor)
        .Select(x => x.Key)
        .FirstAsMaybe(o => new TilePoint(o).Equals(tile));
    public Maybe<GameObject> GetObject(TilePoint tile) => _pieces
        .Where(x => !x.Value.Rules.IsFloor)
        .Select(x => x.Key)
        .FirstAsMaybe(o => new TilePoint(o).Equals(tile));
    public Maybe<GameObject> GetSelectable(TilePoint tile) =>  Selectables.FirstAsMaybe(o => new TilePoint(o).Equals(tile));
    public MapPiece GetPiece(TilePoint tile) => _pieces.FirstOrDefault(p => new TilePoint(p.Key).Equals(tile)).Value.Piece;
    public MapPiece GetFloorPiece(TilePoint tile) => _pieces.FirstOrDefault(p => new TilePoint(p.Key).Equals(tile) && p.Value.Piece != MapPiece.Nothing && p.Value.Rules.IsFloor).Value.Piece;
    public MapPiece GetObjectPiece(TilePoint tile) => _pieces.FirstOrDefault(p => new TilePoint(p.Key).Equals(tile) && !p.Value.Rules.IsFloor).Value.Piece;
    
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
    public bool IsSelectable(TilePoint tile) =>
        _pieces.Any(w => new TilePoint(w.Key).Equals(tile) && w.Value.Rules.IsSelectable);

    public void Refresh() => Notify(() => { });
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
    
    public void Remove(params GameObject[] objs)
    {
        Notify(() =>
        {
            foreach (var obj in objs)
            {
                _destroyedObjects[obj] = _pieces[obj];
                _pieces.Remove(obj);
            }
        });  
    }
    
    public LevelMap GetLevelMap()
    {
        var builder = new LevelMapBuilder(levelName, Mathf.CeilToInt(max.x + 1), Mathf.CeilToInt(max.y + 1));
        _pieces.ForEach(p => builder.With(new TilePoint(p.Key), p.Value.Piece));
        return builder.Build();
    }

    private LevelStateSnapshot GetSnapshot()
    {
        var maxX = 0;
        var maxY = 0;
        var floors = new Dictionary<TilePoint, MapPiece>();
        var pieces = new Dictionary<TilePoint, MapPiece>();
        var piecesCounts = new Dictionary<MapPiece, int>();
        Enum.GetValues(typeof(MapPiece)).Cast<MapPiece>().ForEach(p => piecesCounts[p] = 0);

        foreach (var p in _pieces)
        {
            if (!piecesCounts.ContainsKey(p.Value.Piece))
                piecesCounts[p.Value.Piece] = 0;
            piecesCounts[p.Value.Piece]++;
            var tp = new TilePoint(p.Key);
            if (p.Value.Rules.IsFloor)
                floors[tp] = p.Value.Piece;
            else
                pieces[tp] = p.Value.Piece;
            maxX = Math.Max(maxX, tp.X);
            maxY = Math.Max(maxY, tp.Y);
        }
        
        var counters = _counters.ToDictionary(x => x.Key, x => x.Value);
        counters[CounterType.NumFoodCollected] = counters[CounterType.NumFoodPossible] - piecesCounts[MapPiece.Food];
        counters[CounterType.NumTreatsCollected] = counters[CounterType.NumTreatsPossible] - piecesCounts[MapPiece.Treat];
        // NOTE: This is Wrong. Need to only track Moves made.
        counters[CounterType.NumMovesMade] += 1;

        var levelBounds = new Vector2Int(maxX + 1, maxY + 1);
        return new LevelStateSnapshot(levelBounds, floors, pieces, counters);
    }

    public void ClearSnapshot() => _snapshot = null;
    
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
