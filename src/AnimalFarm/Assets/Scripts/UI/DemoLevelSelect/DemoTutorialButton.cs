using UnityEngine;
using UnityEngine.UI;

public class DemoTutorialButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private CurrentLevel currentLevel;
    [SerializeField] private BoolVariable isLevelStart;

    public void Init(int zoneIndex, Campaign currentCampaign)
    {
        var zone = currentCampaign.Value[zoneIndex];
        gameObject.SetActive(zone.Tutorial.IsPresent);
        if (!zone.Tutorial.IsPresent)
            return;
        button.onClick.AddListener(() =>
        {
            currentLevel.SelectLevel(zone.Tutorial.Value, zoneIndex, -1);
            isLevelStart.Value = true;
            Message.Publish(new StartDemoLevelRequested());
        });
    }
}
