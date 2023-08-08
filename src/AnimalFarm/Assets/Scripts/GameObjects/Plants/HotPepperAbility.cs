using System.Collections;
using UnityEngine;

public class HotPepperAbility : OnMessage<PieceMoved, PieceMovementFinished>
{
    [SerializeField] private GameObject pepper;
    [SerializeField] private CurrentLevelMap map;
    [SerializeField] private UiSfxPlayer sfx;
    [SerializeField] private FloatReference preAnimDelayDuration;
    [SerializeField] private FloatReference preMoveDelayDuration = new FloatReference(0.5f);
    [SerializeField] private CurrentSelectedPiece selectedPiece;

    private int _awaitingMoveFinishedNumber = -1;
    private FireVfx _fire;
    
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
        
        if (_fire != null)
            _fire.Deactivate();
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
        _fire = heroObj.GetComponentInChildren<FireVfx>();
        var heroTile = new TilePoint(heroObj);
        var delta = new TilePoint(gameObject) - heroTile;
        selectedPiece.Deselect();
        Message.Publish(new BeginCameraHighlight(Vector3.zero, map.Hero));
        yield return new WaitForSeconds(preAnimDelayDuration.Value);
        
        if (_fire != null)
            _fire.Activate();
        yield return new WaitForSeconds(preMoveDelayDuration.Value);
        pepper.SetActive(false);
        _awaitingMoveFinishedNumber = msg.MoveNumber;

        movingHero.Move(MovementType.AutoRide, msg.MoveNumber, heroTile, heroTile.Plus(delta * 4), speedFactor: 0.5f);
        yield return new WaitForSeconds(0.1f);
        movingHero.SetAnimation(3);
    }
}
