using UnityEngine;

public class RegisterAsSelectable : MonoBehaviour
{
    [SerializeField] private CurrentLevelMap map;

    private void Awake() => map.RegisterAsSelectable(gameObject);
}
  
