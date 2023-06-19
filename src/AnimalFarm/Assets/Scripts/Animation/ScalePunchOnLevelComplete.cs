
using DG.Tweening;
using UnityEngine;

public class ScalePunchOnLevelComplete : OnMessage<LevelCompleted>
{
    [SerializeField] private float amount;
    [SerializeField] private float duration;
    [SerializeField] private GameObject punchTarget;
    
    protected override void Execute(LevelCompleted msg)
    {
        punchTarget.transform.DOPunchScale(new Vector3(amount, amount, amount), duration, 1);
    }
}
