using System.Linq;
using UnityEngine;

public class OnEnterDolphinPerformRide : OnMessage<PieceMovementFinished>
{
    [SerializeField] private MovingPieceXZ piece;
    [SerializeField] private CurrentLevelMap map;
    [SerializeField] private float rideHeightOffset;
    [SerializeField] private CurrentSelectedPiece selectedPiece;

    protected override void Execute(PieceMovementFinished msg)
    {
        if (msg.MovementType != MovementType.SwimRide)
            return;

        var dolphinExit = map.Snapshot.Pieces.Where(p => p.Value == MapPiece.DolphinRideExit).ToArray();
        if (dolphinExit.Length < 1)
        {
            Log.SInfo(LogScopes.GameFlow, $"No Dolphin Exit Found");
            return;
        }

        // NOTE: Remove Dolphin Exit Piece from Map
        var to = dolphinExit[0].Key;
        var dolphinExitPiece = map.GetObject(dolphinExit[0].Key);
        if (dolphinExitPiece.IsPresent)
            map.Remove(dolphinExitPiece.Value);
        
        var hero = map.Hero;
        var originalHeroParent = hero.transform.parent;
        hero.transform.parent = transform;
        hero.transform.localPosition += new Vector3(0, rideHeightOffset, 0);
        
        var from = new TilePoint(gameObject);
        Log.SInfo(LogScopes.GameFlow, $"Performing Dolphin Ride {from} -> {to}");
        piece.Travel(MovementType.AutoRide, msg.MoveNumber, from, to, () =>
        {
            // NOTE: Remove Dolphin Piece from Map
            map.Remove(gameObject);
            hero.transform.parent = originalHeroParent;
            hero.transform.localPosition -= new Vector3(0, rideHeightOffset, 0);
            map.Move(hero, from, to);
            selectedPiece.Select(hero);
        });
    }
}
