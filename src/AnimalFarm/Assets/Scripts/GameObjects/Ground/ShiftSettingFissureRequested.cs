using UnityEngine;

public class ShiftSettingFissureRequested
{
    public Vector2Int ShiftDirection { get; }
    public int FissureIndex { get; }
    public float Duration { get; }

    public ShiftSettingFissureRequested(Vector2Int shiftDirection, int fissureIndex, float duration)
    {
        ShiftDirection = shiftDirection;
        FissureIndex = fissureIndex;
        Duration = duration;
    }
}
