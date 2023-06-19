using UnityEngine;

public class TileIndicatedProcessor : OnMessage<TileIndicated>
{
    [SerializeField] private CurrentSelectedPiece piece;
    [SerializeField] private CurrentLevelMap map;

    protected override void Execute(TileIndicated msg)
    {
        var selected = piece.Selected;
        if (selected.IsPresent)
        {
            var p = selected.Value;
            var moveRequested = new MoveToRequested(p, new TilePoint(p.gameObject), msg.Tile);
            var movementProposal = MoveProcessor.MovementIsPossible(map, moveRequested);
            if (movementProposal.IsPresent)
            {
                Debug.Log($"Move To {msg.Tile} Requested");
                Message.Publish(moveRequested);
                return;
            }
        }

        var selectable = map.GetSelectable(msg.Tile);
        if (selectable.IsPresent && IsNotAlreadySelected(msg.Tile))
        {
            Debug.Log($"Selected Piece at {msg.Tile}");
            Message.Publish(new PieceSelected(selectable.Value));
        }
    }

    private bool IsNotAlreadySelected(TilePoint tile) => !piece.Selected.IsPresentAnd(p => new TilePoint(p).Equals(tile));
}
