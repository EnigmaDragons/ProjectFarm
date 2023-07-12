using System;

public enum MovementType
{
    None = 0,
    [Obsolete] Walk = 99,
    Jump = 1,
    [Obsolete] Attack = 2,
    [Obsolete] Leap = 3,
    Eat = 4,
    Enter = 5,
    SwimRide = 6,
    AutoRide = 7,
}
