using System.Collections;
using System.Linq;
using UnityEngine;

public class ElephantAbility : OnMessage<PieceMoved>
{
    [SerializeField] private MovingPieceXZ obj;
    [SerializeField] private CurrentLevelMap map;
    [SerializeField] private GameObject waterAbilityGraphics;
    [SerializeField] private FloatReference waterAbilityDuration;

    protected override void Execute(PieceMoved msg)
    {
        if (msg.MovementType != MovementType.Activate || !msg.To.Equals(new TilePoint(gameObject)))
            return;

        var piece = map.GetObjectPiece(msg.To);
        var activateAbility = piece.Rules().ActivationAbility;
        if (activateAbility != ActivationType.WaterWholeWorld)
            return;
        
        if (waterAbilityGraphics != null)
            waterAbilityGraphics.SetActive(true);
        obj.FaceTowards(msg.To - msg.From);
        StartCoroutine(FinishActivation(msg));
    }

    private IEnumerator FinishActivation(PieceMoved msg)
    {
        yield return new WaitForSeconds(waterAbilityDuration.Value);
        if (waterAbilityGraphics != null)
            waterAbilityGraphics.SetActive(false);

        foreach (var seedling in map.Snapshot.Floors.Where(f => f.Value == MapPiece.Seedling))
        {
            var waterTarget = map.GetFloorTile(seedling.Key);
            if (waterTarget.IsPresent)
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
