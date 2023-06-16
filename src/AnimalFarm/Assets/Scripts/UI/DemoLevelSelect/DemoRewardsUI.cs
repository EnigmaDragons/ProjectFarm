using System.Collections;
using UnityEngine;

public sealed class DemoRewardsUI : OnMessage<LevelCompleted, DemoQuitLevelRequested>
{
    [SerializeField] private GameObject ui;
    [SerializeField] private FloatReference delayAmount;
    
    private GameObject _current;

    protected override void Execute(LevelCompleted msg) => StartCoroutine(ShowAfterDelay(delayAmount));

    protected override void Execute(DemoQuitLevelRequested msg)
    {
        if (_current != null)
            Destroy(_current);
    }
    
    private IEnumerator ShowAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        _current = Instantiate(ui);
    }
}
