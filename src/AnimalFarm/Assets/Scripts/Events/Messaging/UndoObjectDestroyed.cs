using UnityEngine;

public sealed class UndoObjectDestroyed
{
    public GameObject Object { get; }
    public TilePoint Location { get; }

    public UndoObjectDestroyed(GameObject o)
    {
        Object = o;
        Location = new TilePoint(o);
    }
}
