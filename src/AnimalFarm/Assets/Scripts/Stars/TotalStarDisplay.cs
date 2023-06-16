using TMPro;
using UnityEngine;

public class TotalStarDisplay : MonoBehaviour
{
    [SerializeField] private SaveStorage storage;
    [SerializeField] private TextMeshProUGUI text;

    private void Start() => text.text = storage.GetTotalStars().ToString();
}
