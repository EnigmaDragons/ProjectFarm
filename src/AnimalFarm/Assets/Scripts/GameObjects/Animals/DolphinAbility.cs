using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DolphinAbility : OnMessage<PieceMovementFinished>
{
    [SerializeField] private MovingPieceXZ piece;
    [SerializeField] private CurrentLevelMap map;
    [SerializeField] private float rideHeightOffset;
    [SerializeField] private CurrentSelectedPiece selectedPiece;
    [SerializeField] private AudioClipWithVolume soundOnActivate;
    [SerializeField] private AudioClipWithVolume dolphinVoiceSound;
    [SerializeField] private AudioSource swimmingSound;
    [SerializeField] private UiSfxPlayer sfx;

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

        Message.Publish(new BeginCameraHighlight(Vector3.zero, gameObject, map.Hero));
        StartCoroutine(PerfomMoveAfterDelay(msg, dolphinExit));
    }

    private IEnumerator PerfomMoveAfterDelay(PieceMovementFinished msg, KeyValuePair<TilePoint, MapPiece>[] dolphinExit)
    {
        sfx.Play(soundOnActivate);
        sfx.Play(dolphinVoiceSound);
        yield return new WaitForSeconds(0.3f);
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
        swimmingSound.Play();
        piece.Travel(MovementType.AutoRide, msg.MoveNumber, from, to, () =>
        {
            // NOTE: Remove Dolphin Piece from Map
            map.Remove(gameObject);
            hero.transform.parent = originalHeroParent;
            hero.transform.localPosition -= new Vector3(0, rideHeightOffset, 0);
            map.Move(hero, from, to);
            selectedPiece.Select(hero);
            swimmingSound.Stop();
            Message.Publish(new EndCameraHighlight());
        });
    }
}
