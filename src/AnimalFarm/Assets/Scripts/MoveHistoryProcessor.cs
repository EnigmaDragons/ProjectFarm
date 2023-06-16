using UnityEngine;

public sealed class MoveHistoryProcessor : MonoBehaviour
{
    [SerializeField] private MoveHistory history;

    private void OnEnable()
    {
        Message.Subscribe<UndoRequested>(_ => history.Undo(), this);
        Message.Subscribe<LevelReset>(_ => history.Reset(), this);
        Message.Subscribe<PieceMoved>(p => history.Add(p), this);
    }

    private void OnDisable()
    {
        Message.Unsubscribe(this);
    }
}
