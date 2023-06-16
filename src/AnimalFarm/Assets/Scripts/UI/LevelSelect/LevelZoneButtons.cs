using UnityEngine;

public sealed class LevelZoneButtons : MonoBehaviour
{
    [SerializeField] private LevelButton[] buttons;

    public void Init(int zoneNumber, GameLevels zone)
    {
        for (var i = 0; i < buttons.Length; i++)
        {
            var button = buttons[i];
            var hasLevel = i < zone.Value.Length;
            button.gameObject.SetActive(hasLevel);
            if (hasLevel)
                button.Init(zoneNumber, i, zone.Value[i]);
        }
    }
}

