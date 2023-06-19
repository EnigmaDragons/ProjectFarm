using UnityEngine;

[CreateAssetMenu]
public sealed class MustMoveCardinally : MovementRestrictionRule
{
    public override bool IsValid(MovementProposed m) => m.Delta.IsCardinal();
}
