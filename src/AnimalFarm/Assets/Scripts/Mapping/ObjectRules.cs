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

    public MovementType[] MovementTypes;
    public bool IsSelectable => MovementTypes != null && MovementTypes.Length > 0;
    
    public MovementType[] MovementTargetTypes;
    
    public bool IsEdible => MovementTargetTypes.Any(mt => mt == MovementType.Eat);
    public bool IsEnterable => MovementTargetTypes.Any(mt => mt == MovementType.Enter);
}
