using DG.Tweening;
using TMPro;
using UnityEngine;

public sealed class ShowJumpablesUI : OnMessage<LevelReset, LevelStateChanged>
{
    [SerializeField] private CurrentLevelMap map;
    [SerializeField] private RectTransform icon;
    [SerializeField] private TextMeshProUGUI counterLabel;

    private int _lastJumpables;
    private int _totalJumpables;
    
    protected override void Execute(LevelReset msg)
    {
        _totalJumpables = map.NumOfJumpables;
        _lastJumpables = _totalJumpables;
        counterLabel.text = $"0/{_totalJumpables}";
    }

    protected override void Execute(LevelStateChanged msg)
    {
        var newNumberOfJumpables = map.NumOfJumpables;
        var collected = _totalJumpables - newNumberOfJumpables;
        counterLabel.text = $"{collected}/{_totalJumpables}";
        
        if (newNumberOfJumpables < _lastJumpables)
        {
            icon.localScale = new Vector3(1f, 1f, 1f);
            icon.DOPunchScale(new Vector3(1.3f, 1.3f, 1.3f), 0.4f, 1);
        }

        _lastJumpables = newNumberOfJumpables;
    }
}
