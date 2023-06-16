using UnityEngine;

public abstract class MovementRestrictionRule : ScriptableObject
{
    public abstract bool IsValid(MovementProposed m);
}
