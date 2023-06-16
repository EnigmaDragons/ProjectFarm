using UnityEngine;

public class SaveStarsAchievedOnLevel : OnMessage<EndingLevelAnimationFinished>
{
    [SerializeField] private CurrentLevelStars stars;
    [SerializeField] private CurrentLevel level;
    [SerializeField] private SaveStorage storage;

    protected override void Execute(EndingLevelAnimationFinished msg)
    {
        storage.SaveStars(level.ActiveLevel, stars.Count);
        Message.Publish(new StarsUpdated());
    } 
}
