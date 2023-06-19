using UnityEngine;

[CreateAssetMenu]
public class MayEat : MovementOptionRule
{
    [SerializeField] private CurrentLevelMap map;
    
    public override MovementType Type => MovementType.Eat;
    
    public override bool IsPossible(MoveToRequested m)
    {
        var isEdible = map.IsEdible(m.To);
        var isOneAway = m.From.IsAdjacentTo(m.To) && m.From.DistanceFrom(m.To) == 1;
        return isEdible && isOneAway;
    }
}
