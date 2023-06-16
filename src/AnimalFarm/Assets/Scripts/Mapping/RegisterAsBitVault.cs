using UnityEngine;

public class RegisterAsBitVault : MonoBehaviour
{
    [SerializeField] private CurrentLevelMap map;

    private void Awake() => map.RegisterBitVault(gameObject);
}
