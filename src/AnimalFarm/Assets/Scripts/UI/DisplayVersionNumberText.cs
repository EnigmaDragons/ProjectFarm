using TMPro;
using UnityEngine;

public sealed class DisplayVersionNumberText : MonoBehaviour
{
    [SerializeField] private string prefix;
    [SerializeField] private StringReference value;
    [SerializeField] private BoolReference betaMode;
    [SerializeField] private BoolReference demoMode;
    [SerializeField] private TextMeshProUGUI text;

    private string Mode => demoMode 
        ? "Demo" 
        : betaMode 
            ? "Demo" 
            : "";
    
    private void Awake() => text.text = $"{prefix}{value} {Mode}".TrimEnd(' ');
}
