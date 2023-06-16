using UnityEngine;

public sealed class LevelSelectUIController : OnMessage<ShowLevelSelect>
{
    [SerializeField] private GameObject target;

    private void Awake() => target.SetActive(false);
    
    protected override void Execute(ShowLevelSelect msg)
    {
        target.SetActive(true);
    }
}
