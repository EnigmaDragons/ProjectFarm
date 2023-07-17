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
    [SerializeField] private FloatReference activateAnimDuration;
    [SerializeField] private FloatReference collapseDuration = new FloatReference(1f);
    [SerializeField] private CurrentSelectedPiece selectedPiece;

    private readonly int _abilityAnim = 1;
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
        var prevSelectedPiece = selectedPiece.Selected;
        selectedPiece.Deselect();
        sfx.Play(soundOnActivate);
        obj.FaceTowards(msg.From - msg.To);
        yield return new WaitForSeconds(preAnimDelayDuration.Value);
        
        _animator.SetInteger("animation", _abilityAnim);
        yield return new WaitForSeconds(activateAnimDuration.Value);
        _animator.SetInteger("animation", 0);

        var fissureTiles = map.Pieces.Where(x => x.Value.Piece == MapPiece.Fissure).ToArray();
        var isColumn = fissureTiles.Select(t => new TilePoint(t.Key).X).Distinct().Count() == 1;
        var fissureLocation = isColumn ? new TilePoint(fissureTiles.First().Key).X : new TilePoint(fissureTiles.First().Key).Y;
        var fissureSettingTiles = isColumn 
            ? map.SettingPieces.Where(p => new TilePoint(p).X == fissureLocation) 
            : map.SettingPieces.Where(p => new TilePoint(p).Y == fissureLocation);
        var allFissureObjs = fissureTiles.Select(f => f.Key).Concat(fissureSettingTiles).ToArray();
        var objsToMove = isColumn
            ? map.Pieces.Where(p => new TilePoint(p.Key).X > fissureLocation).ToArray()
            : map.Pieces.Where(p => new TilePoint(p.Key).Y > fissureLocation).ToArray();
        var settingObjsToMove = isColumn
            ? map.SettingPieces.Where(p => new TilePoint(p).X > fissureLocation).ToArray()
            : map.SettingPieces.Where(p => new TilePoint(p).Y > fissureLocation).ToArray();
        var allObjsToMove = objsToMove.Select(o => o.Key).Concat(settingObjsToMove).ToArray();
        Log.SInfo(LogScopes.Movement, $"Fissure close will move {objsToMove.Length} objs. {string.Join(",", objsToMove.Select(o => new TilePoint(o.Key)))}");
        var translateAmount = isColumn ? new Vector3(-1, 0, 0) : new Vector3(0, 0, -1);
        var scaleAmount = isColumn ? new Vector3(0, 1, 1) : new Vector3(1, 1, 0);
        foreach (var f in allFissureObjs)
            f.transform.DOScale(scaleAmount, collapseDuration);
        foreach (var o in allObjsToMove) 
            o.transform.DOMove(o.transform.position + translateAmount, collapseDuration.Value);
        
        yield return new WaitForSeconds(collapseDuration.Value + 0.1f);
        foreach (var f in allFissureObjs)
            f.gameObject.SetActive(false);
        
        map.Remove(fissureTiles.Select(t => t.Key).Concat(new [] { gameObject }).ToArray());
        map.Refresh();
        if (prevSelectedPiece.IsPresent)
            selectedPiece.Select(prevSelectedPiece.Value);

        Log.SInfo(LogScopes.Movement, $"Activated Dino Ability");
        Message.Publish(new PieceMovementFinished(msg.MovementType, gameObject, msg.MoveNumber));
    }
}
