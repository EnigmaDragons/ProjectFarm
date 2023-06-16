using UnityEngine;

public class PieceSelectionProcessor : OnMessage<PieceSelected>
{
    [SerializeField] private CurrentSelectedPiece piece;

    protected override void Execute(PieceSelected msg) => piece.Select(msg.Piece);
    private void Awake() => piece.Deselect();
}
