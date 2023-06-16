using UnityEngine;

public sealed class MouseRightClickProcessor : MonoBehaviour
{
    [SerializeField] private BoolReference gameInputActive;

    private void Update()
    {
        if (!Input.GetMouseButtonDown(1) || !gameInputActive.Value) return;
        
        Message.Publish(new PieceDeselected());
    }
}
