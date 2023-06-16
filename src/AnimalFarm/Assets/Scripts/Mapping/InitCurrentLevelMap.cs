using UnityEngine;

public class InitCurrentLevelMap : MonoBehaviour
{
    [SerializeField] private CurrentLevelMap currentLevelMap;

    private void Awake()
    {
        currentLevelMap.InitLevel("Uninitialized");
    }
}
