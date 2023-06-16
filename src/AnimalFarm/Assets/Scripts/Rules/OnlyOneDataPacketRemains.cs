
public sealed class OnlyOneDataPacketRemains : VictoryCondition
{
    public override bool HasCompletedLevel(CurrentLevelMap map) => map.NumSelectableObjects <= 1;
}
