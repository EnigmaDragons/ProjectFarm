using UnityEngine;

public sealed class UpdateMapOnPieceMoved : OnMessage<PieceMoved, UndoPieceMoved>
{
    [SerializeField] private CurrentLevelMap map;
    
    protected override void Execute(PieceMoved msg) => map.Move(msg.Piece, msg.From, msg.To);
    protected override void Execute(UndoPieceMoved msg) => map.Move(msg.Piece, msg.From, msg.To);
}
