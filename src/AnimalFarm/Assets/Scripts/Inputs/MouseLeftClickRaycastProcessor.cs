using UnityEngine;

namespace Inputs
{
    public sealed class MouseLeftClickRaycastProcessor : MonoBehaviour
    {
        [SerializeField] private BoolReference gameInputActive;
        
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
                Debug.Log($"Hit Tile {tilePoint} - {obj.name} - {obj.layer}");
                Message.Publish(new TileIndicated(tilePoint));
            }
        }
    }
}
