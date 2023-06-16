using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class ShowCollectedStars : OnMessage<StarCollected, UndoStarCollected, LevelReset>
{
    [SerializeField] private GameObject parent;
    [SerializeField] private GameObject starPrototype;

    private readonly List<GameObject> _stars = new List<GameObject>();

    protected override void Execute(StarCollected msg)
    {
        _stars.Add(Instantiate(starPrototype, parent.transform));
    }

    protected override void Execute(UndoStarCollected msg)
    {
        if (_stars.Any())
        {
            var star = _stars[0];
            _stars.RemoveAt(0);
            Destroy(star);
        }
    }

    protected override void Execute(LevelReset msg)
    {
        while (_stars.Any())
        {
            var star = _stars[0];
            _stars.RemoveAt(0);
            Destroy(star);
        }
    }
}
