using UnityEngine;

public sealed class RegisterAsBlockingObject : MonoBehaviour
{ 
   [SerializeField] private CurrentLevelMap map;

   private void Awake() => map.RegisterBlockingObject(gameObject);
}
