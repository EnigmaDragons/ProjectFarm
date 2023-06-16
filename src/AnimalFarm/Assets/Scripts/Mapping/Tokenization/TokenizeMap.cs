using UnityEngine;

public sealed class TokenizeMap : MonoBehaviour
{
    [SerializeField] private CurrentMapTokenizer map;
    [SerializeField] private CurrentLevel current;
    [SerializeField, TextArea(10, 10)] private string token = "";

    private void Awake() => map.Init(current.ActiveLevel.Name);

    private void Update()
    {
        if (token.Trim().Length < 1)
        {
            token = "Invalid";
            token = map.Token;
        }
    }
}
