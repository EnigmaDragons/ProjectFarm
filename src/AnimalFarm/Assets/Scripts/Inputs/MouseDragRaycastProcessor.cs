using System;
using UnityEngine;

public class MouseDragRaycastProcessor : MonoBehaviour
{
    [SerializeField] private BoolReference gameInputActive;
    [SerializeField] private CurrentSelectedPiece piece;

    private readonly RaycastHit[] _hits = new RaycastHit[100];
    
    private Camera _camera;

    private TilePoint _clickedTile;

    private void Awake() => _camera = Camera.main;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var indicated = GetIndicated();
            if (indicated.IsPresent)
                _clickedTile = indicated.Value;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            var indicated = GetIndicated();
            if (indicated.IsPresent)
            {
                if (!_clickedTile.Equals(indicated.Value) && gameInputActive.Value && piece.Selected.IsPresentAnd(x => new TilePoint(x).Equals(_clickedTile)))
                    Message.Publish(new MoveToRequested(piece.Selected.Value, _clickedTile, GetReleasedTilePoint(indicated.Value)));
            }
        }
    }

    private Maybe<TilePoint> GetIndicated()
    {
        var ray = _camera.ScreenPointToRay(Input.mousePosition);
        var numHits = Physics.RaycastNonAlloc(ray, _hits, 100f);
        for (var i = 0; i < numHits; i++)
        {
            var obj = _hits[i].transform.gameObject;
            var tilePoint = new TilePoint(obj);
            return tilePoint;
        }
        return Maybe<TilePoint>.Missing();
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
