
public sealed class PieceMovementFinished
{
    public MovementType MovementType { get; }

    public PieceMovementFinished(MovementType type)
    {
        MovementType = type;
    }
}
