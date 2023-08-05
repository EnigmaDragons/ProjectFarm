using System.Collections;
using UnityEngine;

public class HotPepperAbility : OnMessage<PieceMoved, PieceMovementFinished>
{
    [SerializeField] private GameObject pepper;
    [SerializeField] private CurrentLevelMap map;
    [SerializeField] private UiSfxPlayer sfx;
    [SerializeField] private FloatReference preAnimDelayDuration;
    [SerializeField] private CurrentSelectedPiece selectedPiece;

    private int _awaitingMoveFinishedNumber = -1;
    
    protected override void Execute(PieceMoved msg)
    {
        if (msg.MovementType != MovementType.Activate || !msg.To.Equals(new TilePoint(gameObject)))
            return;

        var piece = map.GetObjectPiece(msg.To);
        var activateAbility = piece.Rules().ActivationAbility;
        if (activateAbility != ActivationType.EatHotPepper)
            return;
        
        StartCoroutine(FinishActivation(msg));
    }

    protected override void Execute(PieceMovementFinished msg)
    {
        if (msg.MoveNumber != _awaitingMoveFinishedNumber)
            return;
        
        _awaitingMoveFinishedNumber = -1;
        selectedPiece.Select(msg.Object);
        Log.SInfo(LogScopes.Movement, $"Ate Hot Pepper Finished");
        Message.Publish(new PieceMovementFinished(msg.MovementType, gameObject, msg.MoveNumber));
        Message.Publish(new EndCameraHighlight());
    }

    private IEnumerator FinishActivation(PieceMoved msg)
    {
        Log.SInfo(LogScopes.Movement, $"Ate Hot Pepper Started");
        var heroObj = msg.Piece;
        var movingHero = heroObj.GetComponent<MovingPieceXZ>();
        var heroTile = new TilePoint(heroObj);
        var delta = new TilePoint(gameObject) - heroTile;
        Message.Publish(new BeginCameraHighlight(Vector3.zero, map.Hero));
        yield return new WaitForSeconds(preAnimDelayDuration.Value);
        
        selectedPiece.Deselect();
        pepper.SetActive(false);
        _awaitingMoveFinishedNumber = msg.MoveNumber;

        movingHero.Move(MovementType.AutoRide, msg.MoveNumber, heroTile, heroTile.Plus(delta * 4), speedFactor: 0.5f);
    }
}
