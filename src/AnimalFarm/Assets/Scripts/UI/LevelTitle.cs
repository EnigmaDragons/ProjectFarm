using TMPro;
using UnityEngine;

public class LevelTitle : MonoBehaviour
{
    [SerializeField] private CurrentLevel level;
    [SerializeField] private TextMeshProUGUI text;

    private void Awake() => text.text = level.ActiveLevel.Name;
}
