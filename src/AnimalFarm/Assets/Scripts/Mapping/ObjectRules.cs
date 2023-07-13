using System;
using System.Linq;

[Serializable]
public class ObjectRules
{
    public bool IsFloor;
    
    public bool IsWalkable;
    public bool IsJumpable;
    public bool IsBlocking;
    public bool IsCollectible;
    
    public ActivationType ActivationAbility = ActivationType.None;
    public bool IsActivatable => ActivationAbility != ActivationType.None;

    public MovementType[] MovementTypes = new MovementType[0];
    public bool IsSelectable => MovementTypes != null && MovementTypes.Length > 0;
    
    public MovementType[] MovementTargetTypes = new MovementType[0];
    
    public bool IsEdible => MovementTargetTypes != null && MovementTargetTypes.Any(mt => mt == MovementType.Eat);
    public bool IsEnterable => MovementTargetTypes != null && MovementTargetTypes.Any(mt => mt == MovementType.Enter);
    public bool IsSwimRideable =>
        MovementTargetTypes != null && MovementTargetTypes.Any(mt => mt == MovementType.SwimRide);
}
