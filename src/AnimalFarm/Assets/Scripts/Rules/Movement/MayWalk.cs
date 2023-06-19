using System;
using UnityEngine;

public sealed class MayWalk : MovementOptionRule
{
    public override MovementType Type => MovementType.Walk;

    public override bool IsPossible(MoveToRequested m) => (m.Delta.X == 0 && Math.Abs(m.Delta.Y) == 1) || (Math.Abs(m.Delta.X) == 1 && m.Delta.Y == 0);
}
