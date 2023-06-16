using UnityEngine;

namespace Assets.Scripts.UI
{
    public class TutorialIndicatorUI : MonoBehaviour
    {
        [SerializeField] private GameObject[] indicators;
        [SerializeField] private SaveStorage storage;
        [SerializeField] private CurrentZone zone;
        [SerializeField] private GameEvent onZoneChanged;

        private void OnEnable()
        {
            onZoneChanged.Subscribe(EnsurePlayerKnowsAboutTutorials, this);
        }

        private void OnDisable() => onZoneChanged.Unsubscribe(this);

        private void EnsurePlayerKnowsAboutTutorials()
        {
            if (storage.HasVisited(zone.ZoneIndex) || !zone.Zone.Tutorial.IsPresent)
                return;
            indicators.ForEach(x => x.SetActive(true));
            storage.Visit(zone.ZoneIndex);
        }
    }
}
