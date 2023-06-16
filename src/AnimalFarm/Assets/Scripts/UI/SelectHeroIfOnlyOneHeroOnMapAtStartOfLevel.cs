using System.Collections;
using UnityEngine;

public class SelectHeroIfOnlyOneHeroOnMapAtStartOfLevel : OnMessage<LevelReset>
{
    [SerializeField] private CurrentLevelMap map;
    [SerializeField] private float delayDuration = 1.2f;
    private bool _initialized;

    protected override void Execute(LevelReset msg)
    {
        if (_initialized)
            StartCoroutine(ExecuteWithDelay());
        else
        {
            Execute();
            _initialized = true;
        }
    }

    private IEnumerator ExecuteWithDelay()
    {
        yield return new WaitForSeconds(delayDuration);
        Execute();
    }
    
    private void Execute()
    {
        Message.Publish(new PieceSelected(map.Hero));
    }
}
