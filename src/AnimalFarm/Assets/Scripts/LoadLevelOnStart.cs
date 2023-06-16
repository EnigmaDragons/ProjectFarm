using UnityEngine;

public sealed class LoadLevelOnStart : MonoBehaviour
{
    [SerializeField] private GameState state;

    private void Start() => state.InitLevel();
}
