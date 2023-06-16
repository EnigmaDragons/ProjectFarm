public sealed class HeroEnteredTheBitVault : VictoryCondition
{
    public override bool HasCompletedLevel(CurrentLevelMap map) 
        => new TilePoint(map.Hero).Equals(map.BitVaultLocation);
}
