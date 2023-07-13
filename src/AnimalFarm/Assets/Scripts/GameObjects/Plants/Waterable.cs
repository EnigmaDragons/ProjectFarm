using UnityEngine;

public class Waterable : MonoBehaviour
{
    [SerializeField] private GameObject[] hideOnWatered;
    [SerializeField] private GameObject[] showOnWatered;
    [SerializeField] private MapPiece spawnOnWatered;

    private bool _watered;
    
    public void Execute()
    {
        if (_watered)
            return;
        
        hideOnWatered.ForEach(g => g.SetActive(false));
        showOnWatered.ForEach(g => g.SetActive(true));
        Message.Publish(new SpawnMapPieceRequested(spawnOnWatered, new TilePoint(gameObject)));
        
        _watered = true;
    }
    
    public void Revert()
    {
        if (!_watered)
            return;
        
        hideOnWatered.ForEach(g => g.SetActive(true));
        showOnWatered.ForEach(g => g.SetActive(false));
        
        _watered = false;
    }
}
