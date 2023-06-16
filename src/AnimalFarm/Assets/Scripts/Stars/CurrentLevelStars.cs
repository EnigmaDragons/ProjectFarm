using System;
using UnityEngine;

public class CurrentLevelStars : ScriptableObject
{
    [SerializeField] private int count;
    [SerializeField] private GameEvent onStarsChanged;

    public GameEvent OnChanged => onStarsChanged;
    public int Count => count;
    public void Increment() => Notify(() => count++);
    public void Decrement() => Notify(() => count--);
    public void Reset() => Notify(() => count = 0);
    
    private void Notify(Action a) 
    {
        a();
        onStarsChanged.Publish();
    }
}
