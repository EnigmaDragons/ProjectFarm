using TMPro;
using UnityEngine;

public class MoveCounterDisplay : MonoBehaviour
{
    [SerializeField] private CurrentMoveCounter moveCounter;
    [SerializeField] private TextMeshProUGUI text;

    private void Awake() => text.text = $"Moves: {moveCounter.Count}";
}
