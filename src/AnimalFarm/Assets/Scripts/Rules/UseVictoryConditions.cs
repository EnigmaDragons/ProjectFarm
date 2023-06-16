using System.Linq;
using UnityEngine;

public class UseVictoryConditions : OnMessage<LevelStateChanged>
{
    [SerializeField] private CurrentLevelMap map;
    [SerializeField] private VictoryCondition[] conditions;
    [ReadOnly, SerializeField] private bool hasWon;
    
    protected override void Execute(LevelStateChanged msg)
    {
        if (hasWon || map.HasLost || !conditions.All(x => x.HasCompletedLevel(map))) return;
        
        hasWon = true;
        Message.Publish(new LevelCompleted());
    }
}
