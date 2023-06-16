using System.Linq;
using UnityEngine;

public sealed class GainStarIfAllDataNodesRemoved : OnMessage<LevelStateChanged, UndoStarCollected>
{
    [SerializeField] private CurrentLevelMap map;
    [SerializeField] private GameObject collectedStar;

    private bool _awardedStar = false;

    protected override void Execute(LevelStateChanged msg)
    {
        if (!_awardedStar && map.Selectables.Count() == 1)
        {
            _awardedStar = true;
            Message.Publish(StarCollected.NoMoreJumpables);
            var star = Instantiate(collectedStar, map.Hero.transform.parent);
            star.transform.localPosition = map.Hero.transform.localPosition;
        }
    }

    protected override void Execute(UndoStarCollected msg)
    {
        if (msg.StarType.Equals(StarCollected.NoMoreJumpables.StarType))
            _awardedStar = false;
    }
}
