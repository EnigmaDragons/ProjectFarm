using UnityEngine;
using UnityEngine.AI;

public class WanderingNavMeshAi : MonoBehaviour {
 
    [SerializeField] private float wanderRadius;
    [SerializeField] private float initialDelayMin = 0.2f;
    [SerializeField] private float initialDelayMax = 3f;
    [SerializeField] private float wanderTimerMin = 5f;
    [SerializeField] private float wanderTimerMax = 14f;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private int walkAnimation = 1;

    private float _initialDelayRemaining;
    private float _currentWanderRemaining;
    private Animator _animator;
 
    void OnEnable ()
    {
        _initialDelayRemaining = Random.Range(initialDelayMin, initialDelayMax);
        _animator = GetComponentInChildren<Animator>();
    }

    private void SetNewWanderTimer()
    {
        _currentWanderRemaining = Random.Range(wanderTimerMin, wanderTimerMax);
    }

    void Update () {
        if (_initialDelayRemaining > 0) {
            _initialDelayRemaining -= Time.deltaTime;
            return;
        }
        
        _currentWanderRemaining -= Time.deltaTime;
        if (_currentWanderRemaining <= 0) {
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            agent.SetDestination(newPos);
            SetNewWanderTimer();
        }

        if (agent.velocity.magnitude > 0.01) 
            _animator.SetInteger("animation", walkAnimation);
        if (agent.velocity.magnitude < 0.01)
            _animator.SetInteger("animation", 0);
    }
 
    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layerMask) {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;
 
        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layerMask);
 
        return navHit.position;
    }
}
