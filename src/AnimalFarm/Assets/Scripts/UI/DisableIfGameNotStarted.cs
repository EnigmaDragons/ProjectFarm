using UnityEngine;

public class DisableIfGameNotStarted : MonoBehaviour
{
    [SerializeField] private SaveStorage storage;
    [SerializeField] private MonoBehaviour script;

    void Awake() => script.enabled = storage.HasStartedGame();
}
