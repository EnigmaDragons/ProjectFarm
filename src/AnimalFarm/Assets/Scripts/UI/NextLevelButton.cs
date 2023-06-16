using UnityEngine;

namespace Assets.Scripts.UI
{
    public class NextLevelButton : MonoBehaviour
    {
        [SerializeField] private CurrentZone zone;
        [SerializeField] private CurrentLevel level;
        [SerializeField] private Navigator navigator;
        [SerializeField] private GameObject button;
        [SerializeField] private SaveStorage storage;
        [SerializeField] private BoolVariable isLevelStart;
        [SerializeField] private CurrentDialogue currentDialogue;
        [SerializeField] private BoolReference AutoSkipStory;
        [SerializeField] private IsLevelUnlockedCondition isLevelUnlockedCondition;

        private Campaign _campaign => zone.Campaign;

        private void Awake() => button.SetActive(!IsLastLevel && isLevelUnlockedCondition.IsLevelUnlocked(level.ZoneNumber, level.LevelNumber + 1));

        private bool IsLastLevel => zone.Zone.Value.Length == level.LevelNumber + 1;
        private bool IsLastZone => _campaign.Value.Length == level.ZoneNumber + 1;
        private bool IsNextZoneUnlocked => storage.GetTotalStars() >= _campaign.Value[level.ZoneNumber + 1].StarsRequired && storage.GetLevelsCompletedInZone(zone.Zone) == zone.Zone.Value.Length;

        public void Go()
        {
            var nextLevel = level.LevelNumber + 1;
            var gameLevel = _campaign.Value[level.ZoneNumber].Value[nextLevel];
            level.SelectLevel(gameLevel, level.ZoneNumber, nextLevel);
            isLevelStart.Value = true;
            currentDialogue.Set(storage.GetStars(gameLevel) == 0 ? _campaign.Value[level.ZoneNumber].CurrentStory() : new Maybe<ConjoinedDialogues>());
            storage.SaveZone(level.ZoneNumber);
            if (AutoSkipStory.Value || !currentDialogue.Dialogue.IsPresent) 
                navigator.NavigateToGameScene();
            else
                navigator.NavigateToDialogue();
        }
    }
}
