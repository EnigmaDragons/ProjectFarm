using UnityEngine;

[CreateAssetMenu]
public sealed class MayNotMoveToBlocked : MovementRestrictionRule
{
    [SerializeField] private CurrentLevelMap map;

    public override bool IsValid(MovementProposed m) => m.Type == MovementType.Attack || m.Type == MovementType.Eat || !map.IsBlocked(m.To);
}
