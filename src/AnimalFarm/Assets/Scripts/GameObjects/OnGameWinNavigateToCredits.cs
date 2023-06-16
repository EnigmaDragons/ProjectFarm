using UnityEngine;

namespace Assets.Scripts.GameObjects
{
    public class OnGameWinNavigateToCredits : MonoBehaviour
    {
        [SerializeField] private CurrentLevel level;
        [SerializeField] private CurrentZone zone;
        [SerializeField] private SaveStorage storage;
        [SerializeField] private Navigator navigator;

        private void Awake()
        {
            if (level.ZoneNumber == zone.Campaign.Value.Length - 1 && storage.GetLevelsCompletedInZone(zone.Zone) == zone.Zone.Value.Length && !storage.HasWon())
            {
                storage.SaveWin();
                navigator.NavigateToCredits();
            }
        }
    }
}
