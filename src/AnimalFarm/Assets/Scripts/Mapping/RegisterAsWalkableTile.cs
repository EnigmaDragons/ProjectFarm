using UnityEngine;

public class RegisterAsWalkableTile : MonoBehaviour
{
    [SerializeField] private CurrentLevelMap currentLevelMap;

    private void Awake() => currentLevelMap.RegisterWalkableTile(gameObject);
}
