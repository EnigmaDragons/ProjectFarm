using UnityEngine;

public class TileIndicatedProcessor : OnMessage<TileIndicated>
{
    [SerializeField] private CurrentSelectedPiece piece;
    [SerializeField] private CurrentLevelMap map;

    protected override void Execute(TileIndicated msg)
    {
        var selectable = map.GetSelectable(msg.Tile);
        if (selectable.IsPresent && IsNotAlreadySelected(msg.Tile))
        {
            Debug.Log($"Selected Piece at {msg.Tile}");
            Message.Publish(new PieceSelected(selectable.Value));
            return;
        }

        Debug.Log($"Move To {msg.Tile} Requested");
        piece.Selected.IfPresent(p => Message.Publish(new MoveToRequested(p, new TilePoint(p.gameObject), msg.Tile)));
    }

    private bool IsNotAlreadySelected(TilePoint tile) => !piece.Selected.IsPresentAnd(p => new TilePoint(p).Equals(tile));
}
