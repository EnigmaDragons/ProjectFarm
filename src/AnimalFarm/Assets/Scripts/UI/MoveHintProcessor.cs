using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MoveHintProcessor : OnMessage<PieceMovementStarted, PieceMovementFinished, LevelStateChanged>
{
    [SerializeField] private CurrentLevelMap map;
    [SerializeField] private CurrentSelectedPiece piece;
    [SerializeField] private CurrentLevel currentLevel;
    [SerializeField] private GameObject indicatorPrototype;

    private readonly List<GameObject> _hints = new List<GameObject>();

    private void Awake()
    {
        for(var i = 0; i < 20; i++)
            _hints.Add(Instantiate(indicatorPrototype, transform));
    }

    protected override void AfterEnable() => piece.OnChanged.Subscribe(() => UpdateHints(), this);
    protected override void AfterDisable() => piece.OnChanged.Unsubscribe(this);
    protected override void Execute(PieceMovementStarted msg) => HideAllHints();
    protected override void Execute(PieceMovementFinished msg) => UpdateHints();
    protected override void Execute(LevelStateChanged msg) => UpdateHints(msg.After);
    
    private void UpdateHints(LevelStateSnapshot specificSnapshot = null, bool forceClearSnapshot = false)
    {
        HideAllHints();

        if (!piece.Selected.IsPresent)
            return;
        
        if (forceClearSnapshot)
            map.ClearSnapshot();
        var snapshot = specificSnapshot ?? map.Snapshot;
        var obj = piece.Selected.Value;
        var objTile = new TilePoint(obj);
        var mapPiece = snapshot.Pieces.ValueOrDefault(objTile, () => MapPiece.Nothing);
        var movableLocations = snapshot.GetPossibleMoves(objTile).Select(x => x.To).ToArray();
        Log.SInfo(LogScopes.Hints, $"Updating Hints for {mapPiece} {objTile}. Found {movableLocations.Length} options.");
        for (var i = 0; i < movableLocations.Length; i++)
        {
            _hints[i].transform.SetParent(currentLevel.Transform);
            _hints[i].transform.localPosition = new Vector3(movableLocations[i].X, 0, movableLocations[i].Y);
            _hints[i].transform.localRotation = Quaternion.identity;
            _hints[i].transform.parent = transform;
            _hints[i].SetActive(true);
        }
    }

    private void HideAllHints()
    {
        _hints.ForEach(h => h.SetActive(false));
    }

    // NOTE: Not Optimal! Replace with Reactive once race condition is solved.
    private void Update()
    {
        if (_hints.None(h => h.activeSelf) && piece.Selected.IsPresent)
            UpdateHints();
    }
}
