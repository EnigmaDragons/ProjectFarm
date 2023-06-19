using UnityEngine;

public class RegisterObjectRules : MonoBehaviour
{
    [SerializeField] private CurrentLevelMap map;
    [SerializeField] private ObjectRules rules;

    private void Awake()
    {
        if (rules.IsWalkable)
            map.RegisterWalkableTile(gameObject);
        if (rules.IsBlocking)
            map.RegisterBlockingObject(gameObject);
        if (rules.IsEdible)
            map.RegisterAsEdible(gameObject);
        if (rules.IsJumpable)
            map.RegisterAsJumpable(gameObject);
        if (rules.IsCollectible)
            map.RegisterAsCollectible(gameObject);
        if (rules.IsSelectable)
            map.RegisterAsSelectable(gameObject);
    }
}
