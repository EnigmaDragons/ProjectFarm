using UnityEngine;

namespace Inputs
{
    public sealed class MouseLeftClickRaycastProcessor : MonoBehaviour
    {
        [SerializeField] private BoolReference gameInputActive;
        
        private Camera _camera;
        
        private readonly RaycastHit[] _hits = new RaycastHit[100];
        
        private void Awake()
        {
            _camera = Camera.main;
            
        }
        
        private void Update()
        {
            if (!Input.GetMouseButtonDown(0) || !gameInputActive.Value)
                return;
            
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            var numHits = Physics.RaycastNonAlloc(ray, _hits, 100f);
            for (var i = 0; i < numHits; i++)
            {
                if (_hits[i].transform.gameObject.layer != Layers.GameTile && _hits[i].transform.parent.gameObject.layer != Layers.GamePiece)
                    continue;
                

                var hitTransform = _hits[i].transform;
                var obj = hitTransform.gameObject;
                if (obj.layer == Layers.GameTile)
                    obj = hitTransform.parent.gameObject;
                if (obj.layer == Layers.GamePiece)
                    while (obj.layer == Layers.GamePiece && obj.transform.parent != null && obj.transform.parent.gameObject.layer == Layers.GamePiece)
                        obj = obj.transform.parent.gameObject;

                var tilePoint = new TilePoint(obj);
                Log.SInfo(LogScopes.Input, $"Hit Tile {tilePoint} - {obj.name} - {obj.layer}", obj);
                Message.Publish(new TileIndicated(tilePoint));
                return;
            }

            
            Log.SInfo(LogScopes.Input, $"Missed Tile - Num Hits {numHits}");
            for (var i = 0; i < numHits; i++)
            {
                var obj = _hits[i].transform.parent.gameObject;
                Log.SInfo(LogScopes.Input, $"Hit Object - {obj.name} - {obj.layer}", obj);
            }
        }
    }
}
