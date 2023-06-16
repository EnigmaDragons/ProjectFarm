using UnityEngine;

public abstract class MovementOptionRule : ScriptableObject
{
    public abstract MovementType Type { get; }
    public abstract bool IsPossible(MoveToRequested m);
}
