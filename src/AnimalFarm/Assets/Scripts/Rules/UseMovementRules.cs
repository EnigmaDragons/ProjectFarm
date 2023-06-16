using UnityEngine;

public sealed class UseMovementRules : MonoBehaviour
{
    [SerializeField] private CurrentLevelMap map;
    [SerializeField] private MovementOptionRule[] optionRules;
    [SerializeField] private MovementRestrictionRule[] restrictionRules;

    private void Start()
    {
        optionRules.ForEach(r => map.AddMovementOptionRule(r));
        restrictionRules.ForEach(r => map.AddMovementRestrictionRule(r));
    } 
}
