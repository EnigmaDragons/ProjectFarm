using UnityEngine;

public class PlayParticlesOnLevelCompletion : OnMessage<LevelCompleted>
{
    [SerializeField] private GameObject particles;
    [SerializeField] private float _delay;

    private bool _waiting;
    private float _timeWaited;

    protected override void Execute(LevelCompleted msg) => _waiting = true;

    private void Update()
    {
        if (!_waiting)
            return;
        _timeWaited += Time.deltaTime;
        if (_timeWaited >= _delay)
        {
            particles.SetActive(true);
            _waiting = false;
        }
    }
}
