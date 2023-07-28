using UnityEngine;
using UnityEngine.AI;

public class WanderingNavMeshAi : MonoBehaviour {
 
    [SerializeField] private float wanderRadius;
    [SerializeField] private float wanderTimer;
    [SerializeField] private NavMeshAgent agent;
 
    private float _currentWanderDuration;
 
    void OnEnable () {
        _currentWanderDuration = wanderTimer;
    }
 
    void Update () {
        _currentWanderDuration += Time.deltaTime;
 
        if (_currentWanderDuration >= wanderTimer) {
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            agent.SetDestination(newPos);
            _currentWanderDuration = 0;
        }
    }
 
    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layerMask) {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;
 
        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layerMask);
 
        return navHit.position;
    }
}
