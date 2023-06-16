using System.Collections;
using UnityEngine;

public sealed class StartDemoGameLevel : OnMessage<StartDemoLevelRequested, LevelCompleted, DemoQuitLevelRequested, EndingLevelAnimationFinished>
{
    [SerializeField] private GameObject game;
    [SerializeField] private CurrentLevel level;
    [SerializeField] private FloatReference destroyDelay;
    [SerializeField] private BoolVariable gameInputActive;

    private GameObject _current;
    
    protected override void Execute(StartDemoLevelRequested msg)
    {
        gameInputActive.Value = true;
        _current = Instantiate(game);
    }

    protected override void Execute(LevelCompleted msg) => StartCoroutine(ResolveLevelEnd(destroyDelay));
    protected override void Execute(DemoQuitLevelRequested msg) => StartCoroutine(ResolveLevelEnd(0f));
    protected override void Execute(EndingLevelAnimationFinished msg) => StartCoroutine(ResolveLevelEnd(0f));

    private IEnumerator ResolveLevelEnd(float delay)
    {
        yield return new WaitForSeconds(delay);
        KillCurrent();
    }

    private void KillCurrent()
    {
        if (_current == null)
            return;
        
        DestroyImmediate(_current);
        level.Clear();
    }
}
