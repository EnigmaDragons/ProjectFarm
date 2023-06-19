using UnityEngine;

[CreateAssetMenu]
public class MayEnter : MovementOptionRule
{
    [SerializeField] private CurrentLevelMap map;
    
    public override MovementType Type => MovementType.Enter;
    
    public override bool IsPossible(MoveToRequested m)
    {
        var isEnterable = map.IsEnterable(m.To);
        var isOneAway = m.From.IsAdjacentTo(m.To) && m.From.DistanceFrom(m.To) == 1;
        return isEnterable && isOneAway;
    }
}
