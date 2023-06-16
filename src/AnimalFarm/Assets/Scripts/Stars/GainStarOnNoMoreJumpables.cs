using UnityEngine;

public class GainStarOnNoMoreJumpables : OnMessage<LevelStateChanged, UndoStarCollected>
{
    [SerializeField] private CurrentLevelMap map;
    [SerializeField] private GameObject collectedStar;

    private bool _awardedStar = false;

    protected override void Execute(LevelStateChanged msg)
    {
        if (!_awardedStar && map.NumOfJumpables == 0)
        {
            _awardedStar = true;
            Message.Publish(StarCollected.NoMoreJumpables);
            var star = Instantiate(collectedStar, transform.parent.parent);
            star.transform.position = transform.position;
            gameObject.SetActive(false);
        }
    }

    protected override void Execute(UndoStarCollected msg)
    {
        if (msg.StarType.Equals(StarCollected.NoMoreJumpables.StarType))
            _awardedStar = false;
    }
}
