using System;
using UnityEngine;

public class MouseDragProcessor : MonoBehaviour
{
    [SerializeField] private BoolReference gameInputActive;
    [SerializeField] private CurrentSelectedPiece piece;

    private Camera _mainCamera;

    private TilePoint _clickedTile;

    private void Awake() => _mainCamera = Camera.main;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var rawMousePos = Input.mousePosition;
            var adjustedMousePos = rawMousePos + new Vector3(0, 0, -_mainCamera.transform.position.z);
            var mousePos = _mainCamera.ScreenToWorldPoint(adjustedMousePos);
            _clickedTile = new TilePoint(mousePos);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            var rawMousePos = Input.mousePosition;
            var adjustedMousePos = rawMousePos + new Vector3(0, 0, -_mainCamera.transform.position.z);
            var mousePos = _mainCamera.ScreenToWorldPoint(adjustedMousePos);
            var releasedTile = new TilePoint(mousePos);

            if (!_clickedTile.Equals(releasedTile) && gameInputActive.Value && piece.Selected.IsPresentAnd(x => new TilePoint(x).Equals(_clickedTile)))
                Message.Publish(new MoveToRequested(piece.Selected.Value, _clickedTile, GetReleasedTilePoint(releasedTile)));
        }
    }

    private TilePoint GetReleasedTilePoint(TilePoint releasedTilePointRaw)
    {
        var movement = piece.Selected.Value.GetComponent<MovementEnabled>();
        if (movement == null || movement.Types.Count != 1)
            return releasedTilePointRaw;
        var delta = releasedTilePointRaw - _clickedTile;
        if (Math.Abs(delta.X) > Math.Abs(delta.Y))
            delta.Y = 0;
        else if (Math.Abs(delta.X) < Math.Abs(delta.Y))
            delta.X = 0;
        if (Math.Abs(delta.X) + Math.Abs(delta.Y) < 2 || !delta.IsCardinal())
            return releasedTilePointRaw;
        if (delta.X > 0)
            delta.X = 1;
        if (delta.X < 0)
            delta.X = -1;
        if (delta.Y > 0)
            delta.Y = 1;
        if (delta.Y < 0)
            delta.Y = -1;
        if (movement.Types[0] == MovementType.Leap)
        {
            delta.X *= 3;
            delta.Y *= 3;
            return _clickedTile + delta;
        }
        if (movement.Types[0] == MovementType.Jump)
        {
            delta.X *= 2;
            delta.Y *= 2;
            return _clickedTile + delta;
        }
        return _clickedTile + delta;
    }
}
