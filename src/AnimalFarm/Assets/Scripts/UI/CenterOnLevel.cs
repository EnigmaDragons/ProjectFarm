using System.Linq;
using UnityEngine;

public class CenterOnLevel : OnMessage<LevelReset>
{
    [SerializeField] private CurrentLevel level;
    [SerializeField] private Boolean3 dimensions;
    [SerializeField] private Vector3 offset;

    private void Start() => Center();

    protected override void Execute(LevelReset msg) => Center();

    private void Center()
    {
        if (level.ActiveLevel == null)
            return;
        
        var bounds = level.ActiveLevel.Prefab.GetComponentsInChildren<Renderer>().Select(x => x.bounds);
        var boundsCombined = bounds.First();
        bounds.ForEach(x => boundsCombined.Encapsulate(x));
        var x = (dimensions.X ? boundsCombined.center.x : transform.position.x) + offset.x;
        var y = (dimensions.Y ? boundsCombined.center.y : transform.position.y) + offset.y;
        var z = (dimensions.Z ? boundsCombined.center.z : transform.position.z) + offset.z;
        
        transform.position = new Vector3(x, y, z);
    }
}
