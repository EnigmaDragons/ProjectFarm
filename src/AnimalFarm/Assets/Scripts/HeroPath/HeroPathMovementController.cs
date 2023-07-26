using System.Collections.Generic;
using UnityEngine;

public class HeroPathMovementController : OnMessage<PieceMoved, PieceMovementFinished>
{
    [SerializeField] private CurrentSelectedPiece selected;
    [SerializeField] private CurrentLevelMap map;
    [SerializeField] private MovingPieceXZ piece;
    [SerializeField] private UiSfxPlayer sfx;
    [SerializeField] private AudioClipWithVolume activateSound;

    private int _moveNumber;
    private Queue<Vector2Int> _path;
    
    protected override void Execute(PieceMoved msg)
    {
        if (msg.MovementType != MovementType.Genius)
            return;

        Message.Publish(new HeroPathBegun());
        selected.Deselect();
        sfx.Play(activateSound);
        _moveNumber = msg.MoveNumber;
        _path = new Queue<Vector2Int>(map.HeroPath);
        _path.Dequeue(); // Clear the first one, since it's the starting position
        if (_path.Count > 1)
            Message.Publish(new BeginCameraHighlight(new Vector3(0, 0, -0.5f), gameObject));
        MoveNext();
    }

    protected override void Execute(PieceMovementFinished msg) => MoveNext();

    private void MoveNext()
    {
        if (_path == null || _path.Count == 0)
            return;
        
        var next = _path.Dequeue();
        if (_path.Count == 0)
            Message.Publish(new EndCameraHighlight());
        piece.Move(MovementType.Genius, _moveNumber, new TilePoint(piece.gameObject), new TilePoint(next.x, next.y));
    }
}
