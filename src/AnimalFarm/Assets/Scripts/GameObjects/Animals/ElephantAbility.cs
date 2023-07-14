using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class ElephantAbility : OnMessage<PieceMoved>
{
    [SerializeField] private MovingPieceXZ obj;
    [SerializeField] private CurrentLevelMap map;
    [SerializeField] private GameObject waterAbilityGraphics;
    [SerializeField] private FloatReference waterAbilityDuration;
    [SerializeField] private FloatReference preWaterAnimDuration;
    [SerializeField] private FloatReference preAnimDelayDuration;
    [SerializeField] private AudioClipWithVolume soundOnActivate;
    [SerializeField] private AudioClipWithVolume soundOnWater;
    [SerializeField] private UiSfxPlayer sfx;

    private int _abilityAnim = 6;
    private Animator _animator;

    private void Awake() => obj.SetFacingInstant(Facing.Down);

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
        if (activateAbility != ActivationType.WaterWholeWorld)
            return;
        
        StartCoroutine(FinishActivation(msg));
    }

    private IEnumerator FinishActivation(PieceMoved msg)
    {
        sfx.Play(soundOnActivate);
        sfx.Play(soundOnWater);
        obj.FaceTowards(msg.From - msg.To);
        yield return new WaitForSeconds(preAnimDelayDuration.Value);
        _animator.SetInteger("animation", _abilityAnim);
        
        yield return new WaitForSeconds(preWaterAnimDuration.Value);
        if (waterAbilityGraphics != null)
            waterAbilityGraphics.SetActive(true);
        _animator.SetInteger("animation", 0);
        
        yield return new WaitForSeconds(waterAbilityDuration.Value);
        
        foreach (var seedling in map.Snapshot.Floors.Where(f => f.Value == MapPiece.Seedling))
        {
            var waterTarget = map.GetFloorTile(seedling.Key);
            var piece = map.GetObject(seedling.Key);
            if (waterTarget.IsPresent && !piece.IsPresent)
            {
                var objWaterable = waterTarget.Value.GetComponent<Waterable>();
                if (objWaterable != null)
                {
                    objWaterable.Execute();
                }
                else
                {
                    Log.Error($"Seedling is not Waterable");
                }
            }
            else
            {
                Log.Error($"Water Target {seedling.Key} not found");
            }
        }
        
        map.Remove(gameObject);
        map.Refresh();
        Log.SInfo(LogScopes.Movement, $"Activate Elephant Ability - 4");
        Message.Publish(new PieceMovementFinished(msg.MovementType, gameObject, msg.MoveNumber));
    }
}
