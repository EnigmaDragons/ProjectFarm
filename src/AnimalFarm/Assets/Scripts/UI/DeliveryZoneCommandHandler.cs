using UnityEngine;

public sealed class DeliveryZoneCommandHandler : OnMessage<RetryLevel, GoToNextLevel>
{
    [SerializeField] private DeliveryTimeNavigator navigator;
    [SerializeField] private GameLevels levels;
    [SerializeField] private CurrentLevel currentLevel;
    [SerializeField] private DeliveryTimeScoreTracker scoreTracker;
    [SerializeField] private StarCounter counter;
    
    protected override void Execute(RetryLevel msg)
    {
        navigator.NavigateToGameScene();
    }

    protected override void Execute(GoToNextLevel msg)
    {
        var nextLevelNumber = currentLevel.LevelNumber + 1;
        if (levels.Value.Length > nextLevelNumber)
        {
            currentLevel.SelectLevel(levels.Value[nextLevelNumber], 0, nextLevelNumber);
            scoreTracker.RecordLevel(counter.NumStars);
            navigator.NavigateToGameScene();
        }
        else
        {
            scoreTracker.RecordLevel(counter.NumStars);
            navigator.NavigateToSummaryScene();
        }
    }
}
