using System.Collections;
using UnityEngine;

public class EnableForBriefPeriodUponLevelReset : OnMessage<LevelReset>
{
    [SerializeField] private FloatReference duration = new FloatReference(1f);
    [SerializeField] private GameObject target;

    private bool _isRunning;
    private bool _isSetup;

    protected override void Execute(LevelReset msg)
    {
        if (_isSetup && !_isRunning)
            StartCoroutine(Activate());
        _isSetup = true;
    }

    private IEnumerator Activate()
    {
        _isRunning = true;
        target.SetActive(true);
        yield return new WaitForSeconds(duration);
        target.SetActive(false);
        _isRunning = false;
    }
}
