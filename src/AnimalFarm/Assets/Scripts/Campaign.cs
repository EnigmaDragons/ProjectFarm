using UnityEngine;

[CreateAssetMenu]
public class Campaign : ScriptableObject
{
    [SerializeField] private string campaignName;
    [SerializeField] private GameLevels[] value;

    public string Name => campaignName;
    public GameLevels[] Value => value;
}
