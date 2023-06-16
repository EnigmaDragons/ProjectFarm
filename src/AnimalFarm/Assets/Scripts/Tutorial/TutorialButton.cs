using UnityEngine;
using UnityEngine.UI;

public class TutorialButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private CurrentLevel currentLevel;
    [SerializeField] private BoolVariable isLevelStart;
    [SerializeField] private Navigator navigator;
    [SerializeField] private GameObject[] stars;
    [SerializeField] private SaveStorage storage;
    [SerializeField] private CurrentDialogue currentDialogue;

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
            currentDialogue.Set(new Maybe<ConjoinedDialogues>());
            navigator.NavigateToGameScene();
        });
        for (var i = 0; i < stars.Length; i++)
            stars[i].SetActive(storage.GetStars(zone.Tutorial.Value) > i);
    }
}
