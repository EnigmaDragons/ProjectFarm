using UnityEngine;

public class PieceDeselectionProcessor : OnMessage<PieceDeselected>
{
    [SerializeField] private CurrentSelectedPiece piece;

    protected override void Execute(PieceDeselected msg) => piece.Deselect();
}
