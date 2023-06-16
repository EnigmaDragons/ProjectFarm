using System;
using UnityEngine;

[CreateAssetMenu]
public sealed class MoveHistory : ScriptableObject
{
    [SerializeField] private GameEvent onChanged;
    [SerializeField] private IntReference maxUndoDepth;

    private FixedSizeStack<PieceMoved> _moves;
    private FixedSizeStack<PieceMoved> Moves
    {
        get
        {
            if (_moves == null || _moves.Size < maxUndoDepth)
                _moves = new FixedSizeStack<PieceMoved>(maxUndoDepth);
            return _moves;
        }
    }

    public GameEvent OnChanged => onChanged;
    public void Reset() => Notify(() => Moves.Clear());
    public void Add(PieceMoved p) => Notify(() => Moves.Push(p));
    
    public void Undo()
    {
        if (Moves.Count() > 0)
            Notify(() => Moves.Pop().Undo());
    }

    public int Count => Moves.Count();

    private void Notify(Action a)
    {
        a();
        onChanged.Publish();
    }
}
