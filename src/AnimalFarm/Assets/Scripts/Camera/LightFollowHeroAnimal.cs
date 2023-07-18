using UnityEngine;

public class LightFollowHeroAnimal : MonoBehaviour
{
    [SerializeField] private CurrentLevelMap map;
    [SerializeField] private Vector3 offset;
    
    private void LateUpdate()
    {
        if (map.Hero != null)
            transform.localPosition = map.Hero.transform.position + offset;
    }
}
