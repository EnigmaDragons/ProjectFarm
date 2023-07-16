using System.Collections;
using UnityEngine;

public class RotateFissureToBeAligned : OnMessage<LevelReset>
{
    [SerializeField] private CurrentLevelMap map;
    [SerializeField] private GameObject target;

    protected override void Execute(LevelReset msg)
    {
        Refresh();
    }

    private void Refresh()
    {
        var tp = new TilePoint(gameObject);
        var leftTile = map.GetFloorTile(tp.Plus(-1, 0)).IsPresent
            ? map.GetFloorPiece(tp.Plus(new TilePoint(-1, 0)))
            : MapPiece.Nothing;
        var rightTile = map.GetFloorTile(tp.Plus(1, 0)).IsPresent
            ? map.GetFloorPiece(tp.Plus(new TilePoint(1, 0)))
            : MapPiece.Nothing;

        if (leftTile == MapPiece.Fissure || rightTile == MapPiece.Fissure)
        {
            Log.SInfo(LogScopes.MapSetup, $"Rotating fissure at {tp} to be aligned");
            target.transform.localRotation = Quaternion.Euler(0, 90, 0);
        }
        else
        {
            Log.SInfo(LogScopes.MapSetup, $"Not-rotating fissure at {tp}. LeftTile: {leftTile}, RightTile: {rightTile}");
        }
    }
}
