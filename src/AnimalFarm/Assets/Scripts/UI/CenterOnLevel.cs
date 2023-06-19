using System.Linq;
using UnityEngine;

public class CenterOnLevel : OnMessage<LevelReset>
{
    [SerializeField] private CurrentLevel level;
    [SerializeField] private Boolean3 dimensions;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float minHeight;
    [SerializeField] private float maxHeight;
    [SerializeField] private float scaleForWidth = 0.2f;
    
    private void Start() => Center();

    protected override void Execute(LevelReset msg) => Center();

    private void Center()
    {
        if (level.ActiveLevel == null)
            return;
        
        var bounds = level.ActiveLevel.Prefab.GetComponentsInChildren<Renderer>().Where(x => !x.gameObject.tag.Contains("LevelOverflow")).Select(x => x.bounds);
        var boundsCombined = bounds.First();
        bounds.ForEach(x => boundsCombined.Encapsulate(x));
        var x = (dimensions.X ? boundsCombined.center.x : transform.position.x) + offset.x;
        var y = (dimensions.Y ? boundsCombined.center.y : transform.position.y) + offset.y;
        var z = (dimensions.Z ? boundsCombined.center.z : transform.position.z) + offset.z;

        var width = boundsCombined.size.x;
        var upVectorIsY = !dimensions.Y;
        var height = upVectorIsY ? transform.position.y : transform.position.z;
        var heightScale = height * scaleForWidth;
        var scaledHeight = Mathf.Clamp(heightScale * width, minHeight, maxHeight);
        
        var finalY = upVectorIsY ? scaledHeight : y;
        var finalZ = !upVectorIsY ? scaledHeight : z;
        
        transform.position = new Vector3(x, finalY, finalZ);
    }
}
