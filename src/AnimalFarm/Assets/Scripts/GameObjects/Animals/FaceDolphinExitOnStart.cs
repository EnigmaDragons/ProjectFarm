using System.Linq;
using UnityEngine;

public class FaceDolphinExitOnStart : MonoBehaviour
{
    [SerializeField] private CurrentLevelMap map;
    [SerializeField] private MovingPieceXZ piece;

    private void Start()
    {
        var dolphinExit = map.Snapshot.Pieces.Where(p => p.Value == MapPiece.DolphinRideExit).ToArray();
        if (dolphinExit.Length < 1)
        {
            Log.SInfo(LogScopes.MapSetup, $"No Dolphin Exit Found");
            return;
        }

        var dolphinExitPoint = dolphinExit[0].Key;
        var dolphinPoint = new TilePoint(gameObject);
        var delta = dolphinPoint - dolphinExitPoint;
        var newFacing = Facing.Up;
        if (delta.Y > 0)
            newFacing = Facing.Down;
        if (delta.Y < 0)
            newFacing = Facing.Up;
        if (delta.X > 0)
            newFacing = Facing.Left;
        if (delta.X < 0)
            newFacing = Facing.Right;
        piece.SetFacingInstant(newFacing);
        Log.SInfo(LogScopes.MapSetup, $"Dolphin Facing: {newFacing}");
    }
}
