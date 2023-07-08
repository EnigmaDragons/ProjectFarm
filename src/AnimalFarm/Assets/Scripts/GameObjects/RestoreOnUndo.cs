using System.Collections.Generic;
using UnityEngine;

public sealed class RestoreOnUndo : OnMessage<UndoPieceMoved, ObjectDestroyed, LevelReset, PieceJumped>
{
    private readonly Dictionary<int, List<GameObject>> _turnDamagedObjects = new Dictionary<int, List<GameObject>>();
    
    protected override void Execute(UndoPieceMoved msg)
    {
        if (!_turnDamagedObjects.TryGetValue(msg.MoveNumber, out var objs))
            return;

        Debug.Log($"Undo {objs.Count} Destroyed Objects");
        foreach (var obj in objs)
        {
            Debug.Log($"Undo {obj.name}", obj);
            var collectedStartComponent = obj.GetComponent<CollectStarOnEntered>();
            if (collectedStartComponent != null)
            {
                collectedStartComponent.Revert();
                Message.Publish(new UndoObjectDestroyed(obj));
            }

            if (msg.HadEaten(obj))
            {
                var destroyIfEatenComponent = obj.GetComponent<DestroyIfEaten>();
                if (destroyIfEatenComponent != null)
                {
                    destroyIfEatenComponent.Revert();
                    Message.Publish(new UndoObjectDestroyed(obj));
                }
                continue;
            }
            
            if (!msg.HadJumpedOver(obj)) 
                continue;

            var destroyIfJumpedComponent = obj.GetComponent<DestroyIfJumped>();
            if (destroyIfJumpedComponent != null)
            {
                destroyIfJumpedComponent.Revert();
                Message.Publish(new UndoObjectDestroyed(obj));
            }

            var destroyIfJumpedAlt = obj.GetComponent<DestroyIfJumpedNoDeathAnim>();
            if (destroyIfJumpedAlt != null)
            {
                destroyIfJumpedAlt.Revert();
                Message.Publish(new UndoObjectDestroyed(obj));
            }

            var destroyIfDoubleJumpedComponent = obj.GetComponent<DestroyIfDoubleJumped>();
            if (destroyIfDoubleJumpedComponent != null)
            {
                destroyIfDoubleJumpedComponent.Revert();
                Message.Publish(new UndoObjectDestroyed(obj));
            }
        }

        _turnDamagedObjects.Remove(msg.MoveNumber);
    }

    protected override void Execute(ObjectDestroyed msg) => Add(msg.Object, msg.MoveNumber);
    protected override void Execute(LevelReset msg) => _turnDamagedObjects.Clear();
    protected override void Execute(PieceJumped msg) => Add(msg.Piece, msg.MoveNumber);

    private void Add(GameObject o, int moveNumber)
    {
        if (!_turnDamagedObjects.ContainsKey(moveNumber))
            _turnDamagedObjects[moveNumber] = new List<GameObject>();
        _turnDamagedObjects[moveNumber].Add(o);
    }
}

