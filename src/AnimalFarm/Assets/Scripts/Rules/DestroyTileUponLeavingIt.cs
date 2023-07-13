using UnityEngine;

public class DestroyTileUponLeavingIt : OnMessage<PieceMoved>
{
    [SerializeField] private CurrentLevelMap map;
    
    protected override void Execute(PieceMoved msg)
    {
        if (msg.Piece.Equals(gameObject))
            map.GetFloorTile(msg.From).IfPresent(
                t => Message.Publish(new ObjectDestroyed(t, msg.MoveNumber)));
    }
}
