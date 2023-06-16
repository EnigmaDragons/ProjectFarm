using UnityEngine;

public class RegisterAsHero : MonoBehaviour
{
    [SerializeField] private CurrentLevelMap map;

    private void Awake() => map.RegisterHero(gameObject);
}
