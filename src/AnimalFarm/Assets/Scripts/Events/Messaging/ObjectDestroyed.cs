using UnityEngine;

public class ObjectDestroyed
{
    public GameObject Object { get; }
    public int MoveNumber { get; }
    public TilePoint Location { get; }
    public bool IsGameObjectDestructionHandled { get; }

    public ObjectDestroyed(GameObject o, int moveNumber, bool isGameObjectDestructionHandled = false)
    {
        Object = o;
        MoveNumber = moveNumber;
        Location = new TilePoint(o);
        IsGameObjectDestructionHandled = isGameObjectDestructionHandled;
    }
}
