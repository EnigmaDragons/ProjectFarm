using System.Collections;
using UnityEngine;

public class DisableForBriefPeriodUponLevelReset : OnMessage<LevelReset>
{
    [SerializeField] private FloatReference duration = new FloatReference(1f);
    [SerializeField] private GameObject target;

    private bool _isRunning;
    private bool _isSetup;

    protected override void Execute(LevelReset msg)
    {
        if (_isSetup && !_isRunning && target != null)
            StartCoroutine(Activate());
        _isSetup = true;
    }

    private IEnumerator Activate()
    {
        _isRunning = true;
        target.SetActive(false);
        yield return new WaitForSeconds(duration);
        target.SetActive(true);
        _isRunning = false;
    }
}
