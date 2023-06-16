using UnityEngine;

public sealed class RegisterAsFinalCameraAngle : MonoBehaviour
{
    [SerializeField] private CurrentLevelMap map;

    private void OnEnable() => map.RegisterFinalCameraAngle(transform);
}
