using UnityEngine;

public class RegisterAsJumpable : MonoBehaviour
{
    [SerializeField] private CurrentLevelMap map;

    private void Awake() => map.RegisterAsJumpable(gameObject);
}
