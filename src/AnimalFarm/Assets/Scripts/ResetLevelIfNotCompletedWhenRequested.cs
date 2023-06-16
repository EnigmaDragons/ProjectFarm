using System.Collections;
using UnityEngine;

public sealed class ResetLevelIfNotCompletedWhenRequested : MonoBehaviour
{
    [SerializeField] private GameState state;
    [SerializeField] private FloatReference resetDuration;
    [SerializeField] private BoolVariable hasLevelReset;

    private bool _isCompleted;
    private bool _readyToReset = true;

    private void OnEnable()
    {
        Message.Subscribe<LevelResetRequested>(_ => Reset(), this);
        Message.Subscribe<LevelCompleted>(_ => _isCompleted = true, this);
    }

    private void OnDisable() => Message.Unsubscribe(this);

    private void Reset()
    {
        if (!_readyToReset || _isCompleted) return;
        
        StartCoroutine(ResetCooldown());
    }

    private IEnumerator ResetCooldown()
    {
        _readyToReset = false;
        state.InitLevel();
        hasLevelReset.Value = true;
        yield return new WaitForSeconds(resetDuration);
        _readyToReset = true;
    }
}
