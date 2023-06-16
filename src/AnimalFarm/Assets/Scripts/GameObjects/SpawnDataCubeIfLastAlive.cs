using UnityEngine;

public class SpawnDataCubeIfLastAlive : OnMessage<LevelStateChanged>
{
    [SerializeField] private GameObject dataCube;
    [SerializeField] private CurrentLevelMap map;

    private bool _spawned = false;

    protected override void Execute(LevelStateChanged msg)
    {
        if (map.NumOfJumpables == 1 && !_spawned && map.IsJumpable(new TilePoint(gameObject)))
        {
            Instantiate(dataCube, transform);
            _spawned = true;
        }
    }
}
