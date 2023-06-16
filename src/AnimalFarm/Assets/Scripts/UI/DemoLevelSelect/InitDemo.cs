using UnityEngine;

public sealed class InitDemo : MonoBehaviour
{
    [SerializeField] private Campaign demoCampaign;
    [SerializeField] private Theme demoTheme;
    [SerializeField] private CurrentTheme currentTheme;
    [SerializeField] private CurrentZone currentZone;

    private void Awake()
    {
        currentTheme.Set(demoTheme);
        currentZone.Init(demoCampaign);
    }
}
