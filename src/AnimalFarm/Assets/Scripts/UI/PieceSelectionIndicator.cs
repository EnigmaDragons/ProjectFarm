using UnityEngine;

public sealed class PieceSelectionIndicator : OnMessage<PieceSelected, PieceDeselected>
{
    [SerializeField] private CurrentLevelMap map;
    [SerializeField] private GameObject indicator;
    [SerializeField] private GameObject canSelectIndicator;
    [SerializeField] private bool beginSelected;

    private void Start() => Reset();

    private bool GetIsSelectable() => map.IsSelectable(new TilePoint(transform.position));

    protected override void Execute(PieceSelected msg)
    {
        indicator.SetActive(msg.Piece.Equals(gameObject));
        if (canSelectIndicator != null)
            canSelectIndicator.SetActive(GetIsSelectable() && !msg.Piece.Equals(gameObject));
    }

    protected override void Execute(PieceDeselected msg) => Reset();

    private void Reset()
    {
        indicator.SetActive(false);
        if (canSelectIndicator != null)
            canSelectIndicator.SetActive(GetIsSelectable());
        if (beginSelected)
            Message.Publish(new PieceSelected(gameObject, suppressSound: true));
    }
}
