using UnityEngine;

public class AnimalFarmCommandHandler : OnMessage<RetryLevel, GoToNextLevel>
{
    [SerializeField] private GameObject[] hideOnRetry;

    protected override void Execute(RetryLevel msg)
    {
        hideOnRetry.ForEach(g => g.SetActive(false));
        EventPublisher.RetryLevel();
    }

    protected override void Execute(GoToNextLevel msg)
    {
        Navigator.ReloadGameSceneSync();
    }
}
