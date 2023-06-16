using System.Collections.Generic;
using UnityEngine;

public class StarCollectionProcessor : OnMessage<StarCollected, UndoStarCollected, PieceMoved, UndoPieceMoved, LevelReset>
{
    [SerializeField] private CurrentLevelStars stars;

    private readonly Stack<List<StarCollected>> _starTurnHistory = new Stack<List<StarCollected>>();
    private List<StarCollected> _lastTurnStars = new List<StarCollected>();
    
    protected override void Execute(PieceMoved msg)
    {
        _starTurnHistory.Push(_lastTurnStars);
        _lastTurnStars = new List<StarCollected>();
    }

    protected override void Execute(StarCollected msg)
    {
        stars.Increment();
        _lastTurnStars.Add(msg);
    }

    protected override void Execute(UndoStarCollected msg) => stars.Decrement();
    
    protected override void Execute(UndoPieceMoved msg)
    {
        _lastTurnStars.ForEach(x => x.Undo());
        _lastTurnStars = _starTurnHistory.Pop();
    }

    protected override void Execute(LevelReset msg)
    {
        _lastTurnStars.Clear();
        _starTurnHistory.Clear();
    }
}
