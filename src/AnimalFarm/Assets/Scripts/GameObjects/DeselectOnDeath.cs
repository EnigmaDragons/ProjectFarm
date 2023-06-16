using UnityEngine;

public class DeselectOnDeath : MonoBehaviour
{
    [SerializeField] private CurrentSelectedPiece piece;

    private void OnDisable()
    {
        piece.Selected.IfPresent(p =>
        {
            if (gameObject == p)
                Message.Publish(new PieceDeselected());
        });
    }
}
