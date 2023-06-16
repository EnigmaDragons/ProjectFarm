using UnityEngine;

public sealed class UseDefaultMovementRules : OnMessage<LevelReset>
{
    [SerializeField] private CurrentLevelMap map;
    [SerializeField] private MovementOptionRule[] optionRules;
    [SerializeField] private MovementRestrictionRule[] restrictionRules;

    protected override void Execute(LevelReset msg)
    {
        optionRules.ForEach(r => map.AddMovementOptionRule(r));
        restrictionRules.ForEach(r => map.AddMovementRestrictionRule(r));
    }
}
