using UnityEngine;

public class GainStarOnCounterMatch : OnMessage<LevelStateChanged, UndoStarCollected>
{
    [SerializeField] private CounterType counter1;
    [SerializeField] private CounterType counter2;

    private bool _awardedStar = false;
    private const int CounterDefault = 0;

    protected override void Execute(LevelStateChanged msg)
    {
        if (_awardedStar)
            return;
        
        var maybeStar = StarCollected.ForCounterType(counter1);
        if (!maybeStar.IsPresent)
            return;
        
        var star = maybeStar.Value;
        var val1 = msg.After.Counters.ValueOrDefault(counter1, CounterDefault);
        var val2 = msg.After.Counters.ValueOrDefault(counter2, CounterDefault);
        if (val1 != 0 && val1 == val2)
        {
            _awardedStar = true;
            Log.SInfo(LogScopes.Stars, $"Awarded star {star.StarType} for match of {counter1}: " + val1 + $" and {counter2}:" + val2);
            Message.Publish(star);
        }
    }

    protected override void Execute(UndoStarCollected msg)
    {
        var s = StarCollected.ForCounterType(counter1);
        if (s.IsPresent && msg.StarType.Equals(s.Value.StarType))
        {
            Log.SInfo(LogScopes.Stars, $"Undo awarded star {s.Value.StarType}");
            _awardedStar = false;
        }
    }
}
