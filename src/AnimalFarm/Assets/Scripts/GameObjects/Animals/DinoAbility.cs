using System.Collections;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class DinoAbility : OnMessage<PieceMoved>
{
    [SerializeField] private MovingPieceXZ obj;
    [SerializeField] private CurrentLevelMap map;
    [SerializeField] private AudioClipWithVolume soundOnActivate;
    [SerializeField] private UiSfxPlayer sfx;
    [SerializeField] private FloatReference preAnimDelayDuration;
    [SerializeField] private FloatReference collapseDuration = new FloatReference(1f);
    
    private Animator _animator;
    
    private void Start()
    {
        if (_animator == null)
            _animator = gameObject.GetComponentInChildren<Animator>();
    }

    protected override void Execute(PieceMoved msg)
    {
        if (msg.MovementType != MovementType.Activate || !msg.To.Equals(new TilePoint(gameObject)))
            return;

        var piece = map.GetObjectPiece(msg.To);
        var activateAbility = piece.Rules().ActivationAbility;
        if (activateAbility != ActivationType.CloseFissure)
            return;
        
        StartCoroutine(FinishActivation(msg));
    }

    private IEnumerator FinishActivation(PieceMoved msg)
    {
        sfx.Play(soundOnActivate);
        obj.FaceTowards(msg.From - msg.To);
        yield return new WaitForSeconds(preAnimDelayDuration.Value);

        var fissureTiles = map.Pieces.Where(x => x.Value.Piece == MapPiece.Fissure).ToArray();
        var isColumn = fissureTiles.Select(t => new TilePoint(t.Key).X).Distinct().Count() == 1;
        var fissureLocation = isColumn ? new TilePoint(fissureTiles.First().Key).X : new TilePoint(fissureTiles.First().Key).Y;
        var objsToMove = isColumn
            ? map.Pieces.Where(p => new TilePoint(p.Key).X > fissureLocation).ToArray()
            : map.Pieces.Where(p => new TilePoint(p.Key).Y > fissureLocation).ToArray();
        Log.SInfo(LogScopes.Movement, $"Fissure close will move {objsToMove.Length} objs. {string.Join(",", objsToMove.Select(o => new TilePoint(o.Key)))}");
        var translateAmount = isColumn ? new Vector3(-1, 0, 0) : new Vector3(0, 0, -1);
        // TODO: Animate
        foreach (var f in fissureTiles) 
            f.Key.SetActive(false);
        foreach (var o in objsToMove) 
            o.Key.transform.DOMove(o.Key.transform.position + translateAmount, collapseDuration.Value);
        
        yield return new WaitForSeconds(collapseDuration.Value + 0.1f);
        map.Remove(fissureTiles.Select(t => t.Key).Concat(new [] { gameObject }).ToArray());
        map.Refresh();

        Log.SInfo(LogScopes.Movement, $"Activated Dino Ability");
        Message.Publish(new PieceMovementFinished(msg.MovementType, gameObject, msg.MoveNumber));
    }
}
