using UnityEngine;

public class SpawnDataCubeIfOnlyOneSubroutine : OnMessage<LevelStateChanged, UndoPieceMoved, LevelReset>
{
    [SerializeField] private GameObject dataCube;
    [SerializeField] private CurrentLevelMap map;

    private bool _spawned;
    private GameObject _cube;

    protected override void Execute(LevelStateChanged msg)
    {
        if (map.NumOfJumpables == 1 && !_spawned)
        {
            var heroTile = new TilePoint(map.Hero.gameObject);
            if (map.IsJumpable(new TilePoint(heroTile.X - 1, heroTile.Y)) && map.IsWalkable(new TilePoint(heroTile.X - 2, heroTile.Y)))
                SpawnDataCube(new TilePoint(heroTile.X - 2, heroTile.Y));
            if (map.IsJumpable(new TilePoint(heroTile.X + 1, heroTile.Y)) && map.IsWalkable(new TilePoint(heroTile.X + 2, heroTile.Y)))
                SpawnDataCube(new TilePoint(heroTile.X + 2, heroTile.Y));
            if (map.IsJumpable(new TilePoint(heroTile.X, heroTile.Y - 1)) && map.IsWalkable(new TilePoint(heroTile.X, heroTile.Y - 2)))
                SpawnDataCube(new TilePoint(heroTile.X, heroTile.Y - 2));
            if (map.IsJumpable(new TilePoint(heroTile.X, heroTile.Y + 1)) && map.IsWalkable(new TilePoint(heroTile.X, heroTile.Y + 2)))
                SpawnDataCube(new TilePoint(heroTile.X, heroTile.Y + 2));
        }
    }

    protected override void Execute(UndoPieceMoved msg)
    {
        if (map.NumOfJumpables == 1 && _spawned)
        {
            Destroy(_cube);
            _spawned = false;
        }
    }

    protected override void Execute(LevelReset msg)
    {
        if (_spawned)
        {
            Destroy(_cube);
            _spawned = false;
        }
    }

    private void SpawnDataCube(TilePoint position)
    {
        _cube = Instantiate(dataCube);
        _cube.transform.localPosition = new Vector3(position.X, position.Y, _cube.transform.localPosition.z);
        _spawned = true;
    }
}
