
using UnityEngine;

public sealed class StarCounter : OnMessage<StarCollected, UndoStarCollected, LevelReset>
{
    [SerializeField] private int numStars = 0;

    public int NumStars => numStars;

    protected override void Execute(StarCollected msg) => numStars++;
    protected override void Execute(UndoStarCollected msg) => numStars--;
    protected override void Execute(LevelReset msg) => numStars = 0;
}
