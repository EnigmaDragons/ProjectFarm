using UnityEngine;

public abstract class VictoryCondition : ScriptableObject
{
    public abstract bool HasCompletedLevel(CurrentLevelMap map);
}
