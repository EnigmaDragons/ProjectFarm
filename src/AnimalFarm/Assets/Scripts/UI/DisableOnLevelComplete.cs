using UnityEngine;

public sealed class DisableOnLevelComplete : OnMessage<LevelCompleted>
{
    [SerializeField] private GameObject[] targets;

    protected override void Execute(LevelCompleted msg) => targets.ForEach(t => t.SetActive(false));
}
