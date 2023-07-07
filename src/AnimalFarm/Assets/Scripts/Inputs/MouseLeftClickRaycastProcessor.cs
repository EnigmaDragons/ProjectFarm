using UnityEngine;

namespace Inputs
{
    public sealed class MouseLeftClickRaycastProcessor : MonoBehaviour
    {
        [SerializeField] private BoolReference gameInputActive;
        [SerializeField] private BoolReference debugInput;
        
        private Camera _camera;
        private const int TileLayer = 8;
        
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
                if (_hits[i].transform.gameObject.layer != TileLayer)
                    continue;
                
                var obj = _hits[i].transform.parent.gameObject;
                var tilePoint = new TilePoint(obj);
                if (debugInput.Value)
                    Debug.Log($"Hit Tile {tilePoint} - {obj.name} - {obj.layer}");
                Message.Publish(new TileIndicated(tilePoint));
                return;
            }

            if (!debugInput.Value)
                return;
            
            Debug.Log($"Missed Tile - Num Hits {numHits}");
            for (var i = 0; i < numHits; i++)
            {
                var obj = _hits[i].transform.parent.gameObject;
                Debug.Log($"Hit Object - {obj.name} - {obj.layer}", obj);
            }
        }
    }
}
