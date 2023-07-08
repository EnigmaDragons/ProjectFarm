using System.Collections;
using UnityEngine;

public sealed class ResetLevelWhenRequested : MonoBehaviour
{
    [SerializeField] private FloatReference resetDuration;
    [SerializeField] private BoolVariable hasLevelReset;

    private bool _readyToReset = true;

    private void OnEnable()
    {
        Message.Subscribe<LevelResetRequested>(_ => Reset(), this);
    }

    private void OnDisable() => Message.Unsubscribe(this);

    private void Reset()
    {
        if (!_readyToReset) return;
        
        StartCoroutine(ResetWithCooldown());
    }

    private IEnumerator ResetWithCooldown()
    {
        Log.SInfo(LogScopes.GameFlow, "Reset Level Begun");
        _readyToReset = false;
        Message.Publish(new LevelResetApproved());
        hasLevelReset.Value = true;
        Log.SInfo(LogScopes.GameFlow, "Reset Level Finished");
        yield return new WaitForSeconds(resetDuration);
        _readyToReset = true;
        Log.SInfo(LogScopes.GameFlow, "Reset Cooldown Finished");
    }
}
