using System;

[Serializable]
public class GenAnalysisResult
{
    public float AnalysisEngineVersion = 0.1f;
    
    public bool StandardWinPossible;
    public bool GeniusWinPossible;
    
    public int MaxPossibleMoves;
    public int TotalTilesCount;
    public int UnneededTilesCount;
    public int GameLosingMistakeCount;
    public int MinMovesNeededToSolve;
    public int MaxMovesNeededToSolve;
    public int MechanicComplexity;
    
    public int ComplexityScore;
    public int ComplexityDifficulty;
}
