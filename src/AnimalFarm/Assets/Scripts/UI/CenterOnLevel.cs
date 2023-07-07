using System.Collections;
using System.Linq;
using UnityEngine;

public class CenterOnLevel : OnMessage<LevelReset, CenterOnLevelRequested>
{
    [SerializeField] private CurrentLevel level;
    [SerializeField] private Boolean3 dimensions;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float minHeight;
    [SerializeField] private float maxHeight;
    [SerializeField] private float baseHeight = 12;
    [SerializeField] private float scaleForWidth = 0.2f;
    [SerializeField] private float scaleForLength = 0.2f;

    private void Start() => BeginCenterAsync();

    protected override void Execute(LevelReset msg) => BeginCenterAsync();
    protected override void Execute(CenterOnLevelRequested msg) => BeginCenterAsync();

    private void BeginCenterAsync() => StartCoroutine(CenterAsync());

    private IEnumerator CenterAsync()
    {
        yield return new WaitForEndOfFrame();
        Center();
    }
    
    private void Center()
    {
        if (level.ActiveMap == null)
            return;

        var components = level.ActiveMapTransform.GetComponentsInChildren<Renderer>();
        
        var bounds = components
            .Where(x => x.gameObject.layer == Layers.TileLayer)
            .Where(x => !x.gameObject.tag.Contains("LevelOverflow") && !x.gameObject.tag.Contains("ExcludeFromLevelBounds"))
            .Select(x => x.bounds);
        var boundsCombined = bounds.Aggregate(bounds.First(), (bc, b) =>
        {
            bc.Encapsulate(b);
            return bc;
        });
        var x = (dimensions.X ? boundsCombined.center.x : transform.position.x) + offset.x;
        var y = (dimensions.Y ? boundsCombined.center.y : transform.position.y) + offset.y;
        var z = (dimensions.Z ? boundsCombined.center.z : transform.position.z) + offset.z;

        var upVectorIsY = !dimensions.Y;
        var width = boundsCombined.size.x;
        var length = upVectorIsY ? boundsCombined.size.z : boundsCombined.size.y;
        var height = baseHeight;
        var widthHeightScale = height * scaleForWidth * width;
        var lengthHeightScale = height * scaleForLength * length;
        var smalledUsableScale = Mathf.Max(widthHeightScale, lengthHeightScale);
        var scaledHeight = Mathf.Clamp(smalledUsableScale, minHeight, maxHeight);
        
        var finalY = upVectorIsY ? scaledHeight : y;
        var finalZ = !upVectorIsY ? scaledHeight : z;
        
        transform.position = new Vector3(x, finalY, finalZ);
        transform.LookAt(boundsCombined.center);
        Debug.Log($"Camera Center - {bounds.Count()} components. Center: {boundsCombined.center}. Size: {boundsCombined.size}. " +
                  $"WidthHeightScale: {widthHeightScale}. LengthHeightScale: {lengthHeightScale}. SmalledUsableScale: {smalledUsableScale}. ScaledHeight: {scaledHeight}. MinHeight: {minHeight}. MaxHeight: {maxHeight}");
    }
}
