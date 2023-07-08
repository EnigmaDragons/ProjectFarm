using DG.Tweening;
using TMPro;
using UnityEngine;

public class CounterLabel : OnMessage<LevelReset, LevelStateChanged>
{
    [SerializeField] private CurrentLevelMap map;
    [SerializeField] private RectTransform icon;
    [SerializeField] private TextMeshProUGUI counterLabel;
    [SerializeField] private CounterType counterType;
    [SerializeField] private CounterType possibleCounterType;

    private int _lastCounterVal;
    private int _possibleCounterVal;

    protected override void Execute(LevelReset msg)
    {
        _possibleCounterVal = map[possibleCounterType];
        _lastCounterVal = map[counterType];
        
        counterLabel.text = $"0/{_possibleCounterVal}";
    }

    protected override void Execute(LevelStateChanged msg)
    {
        var newVal = msg.After.Counters[counterType];
        counterLabel.text = $"{newVal}/{_possibleCounterVal}";

        if (newVal < _lastCounterVal)
        {
            icon.localScale = new Vector3(1f, 1f, 1f);
            icon.DOPunchScale(new Vector3(1.3f, 1.3f, 1.3f), 0.4f, 1);
        }

        _lastCounterVal = newVal;
    }
}
