using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MoveHintProcessor : OnMessage<PieceMovementStarted, PieceMovementFinished>
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

    protected override void AfterEnable()
    {
        piece.OnChanged.Subscribe(UpdateHints, this);
    }

    protected override void AfterDisable()
    {
        piece.OnChanged.Unsubscribe(this);
    }
    protected override void Execute(PieceMovementStarted msg) => HideAllHints();
    protected override void Execute(PieceMovementFinished msg) => UpdateHints();
    
    private void UpdateHints()
    {
        HideAllHints();

        if (!piece.Selected.IsPresent)
            return;

        var obj = piece.Selected.Value;
        var movableLocations = map.Snapshot.GetPossibleMoves(new TilePoint(obj)).Select(x => x.To).ToArray();
        
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
}
