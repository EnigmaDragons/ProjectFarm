using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public sealed class DisplaySettings : ScriptableObject
{
    [SerializeField] private bool isFullscreen = true;
    [SerializeField] private Vector2Int screenSize;
    
    public bool IsFullscreen => isFullscreen;
    private string Mode => (isFullscreen ? "Fullscreen" : "Windowed");

    public Vector2Int GetInitializedScreenSize()
    {
        screenSize = new Vector2Int(Screen.width, Screen.height);
        isFullscreen = Screen.fullScreen;
        if (screenSize.x == 0 && screenSize.y == 0)
            SetResolution(Screen.resolutions
                .Where(x => x.width % 16 == 0 && x.height % 9 == 0)
                .Where(x => x.width > 800)
                .Reverse()
                .Last());
        return screenSize;
    }

    public void SetFullscreen(bool on) => UpdateAfter(() => isFullscreen = on);
    public void ToggleFullscreen() => UpdateAfter(() => isFullscreen = !isFullscreen);
    public void SetResolution(Resolution r) => UpdateAfter(() => screenSize = new Vector2Int(r.width, r.height));
    
    private void UpdateAfter(Action set)
    {
        var old = $"{Mode}-{screenSize.x}x{screenSize.y}";
        set();
        var newHash = $"{Mode}-{screenSize.x}x{screenSize.y}";
        if (screenSize.x == 0 || screenSize.y == 0)
            Debug.LogError("Cannot set screen size to 0");
        else if (newHash != old)
        {
            Screen.SetResolution(screenSize.x, screenSize.y, isFullscreen);
            Debug.Log($"Changed Display/ Old: {old}. New: {newHash}");
        }
    }
}
