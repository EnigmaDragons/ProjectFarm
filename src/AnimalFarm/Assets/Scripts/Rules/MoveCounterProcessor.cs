using UnityEngine;

public class MoveCounterProcessor : OnMessage<PieceMoved>
{
    [SerializeField] private CurrentMoveCounter moveCounter;

    protected override void Execute(PieceMoved msg) => moveCounter.Increment();
}
