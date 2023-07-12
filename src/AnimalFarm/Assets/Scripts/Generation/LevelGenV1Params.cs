using System;

[Serializable]
public class LevelGenV1Params
{
    public int MinMoves = 2;
    public int MaxMoves = 12;
    public int MaxConsecutiveMisses = 24;

    public int MaxNumGenRetries = 1;
    public bool SkipAnalysis = false;
    public bool SkipPersist = false;

    public MapPiece[] MustInclude = new MapPiece[0]; 
    
    public Permissible Validate()
    {
        if (MinMoves < 1)
            return new Permissible("MinMoves must be at least 2");
        if (MaxMoves < MinMoves)
            return new Permissible("MaxMoves must be at least MinMoves");
        return new Permissible();
    }
}
