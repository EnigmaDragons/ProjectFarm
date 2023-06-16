using UnityEngine;

public sealed class PieceSelectionIndicator : OnMessage<PieceSelected, PieceDeselected>
{
    [SerializeField] private GameObject indicator;
    [SerializeField] private GameObject canSelectIndicator;

    private void Start()
    {
        indicator.SetActive(false);
        if (canSelectIndicator != null)
            canSelectIndicator.SetActive(true);
    }
    
    protected override void Execute(PieceSelected msg)
    {
        indicator.SetActive(msg.Piece.Equals(transform.gameObject));
        if (canSelectIndicator != null)
            canSelectIndicator.SetActive(!msg.Piece.Equals(transform.gameObject));
    }

    protected override void Execute(PieceDeselected msg)
    {
        indicator.SetActive(false);
        if (canSelectIndicator != null)
            canSelectIndicator.SetActive(true);
    }
}
