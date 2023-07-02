using UnityEngine;

public class ChangeMaterialIfNextToGoal : MonoBehaviour
{
    [SerializeField] private CurrentLevelMap map;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material swapTo;

    private void Start()
    {
        if (map.BarnLocation.IsAdjacentTo(new TilePoint(gameObject)))
            meshRenderer.material = swapTo;
    }
}
