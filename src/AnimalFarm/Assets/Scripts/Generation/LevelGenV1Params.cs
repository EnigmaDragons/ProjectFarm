using System;
using UnityEngine;

[Serializable]
public class LevelGenV1Params
{
    public int MinMoves = 2;
    public int MaxMoves = 12;
    public int MaxConsecutiveMisses = 24;

    public int MaxNumGenRetries = 1;
    public bool SkipAnalysis = false;
    public bool SkipPersist = false;
    public bool SkipG = false;
    public float ContinuationOdds = 0.6f;

    public MapPiece[] MustInclude = new MapPiece[0]; 
    
    [Header("Optimization")]
    public bool SkipOptimization = false;
    
    public Permissible Validate()
    {
        if (MinMoves < 1)
            return new Permissible("MinMoves must be at least 2");
        if (MaxMoves < MinMoves)
            return new Permissible("MaxMoves must be at least MinMoves");
        return new Permissible();
    }
}
