using System;
using System.Collections;
using UnityEngine;

public sealed class EnableOnLevelCompleted : OnMessage<LevelCompleted>
{
    [SerializeField] private GameObject target;
    [SerializeField] private FloatReference delay = new FloatReference(0);

    private void Awake() => target.SetActive(false);

    protected override void Execute(LevelCompleted msg) 
        => StartCoroutine(ExecuteAfterDelay(delay, () => target.SetActive(true)));

    private IEnumerator ExecuteAfterDelay(float delayDuration, Action action)
    {
        yield return new WaitForSeconds(delayDuration);
        action();
    }
}
