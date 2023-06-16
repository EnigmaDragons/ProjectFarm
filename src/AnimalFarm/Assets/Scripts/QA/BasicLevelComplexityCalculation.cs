using UnityEngine;

public class BasicLevelComplexityCalculation : OnMessage<LevelReset>
{
    [SerializeField] private CurrentLevelMap map;
    [SerializeField, ReadOnly] private string levelName;
    [SerializeField, ReadOnly] private int difficulty;
    [SerializeField, ReadOnly] private int rawScore;
    [SerializeField, ReadOnly] private int totalJumps;
    [SerializeField, ReadOnly] private int numSpaces;
    [SerializeField, ReadOnly] private int pieceComplexity;
    
    protected override void Execute(LevelReset msg) => Calculate();
    
    private void Calculate()
    {
        var levelMap = map.GetLevelMap();
        var stats = levelMap.BasicComplexityStats();
        levelName = stats.LevelName;
        difficulty = stats.Difficulty;
        rawScore = stats.Score;
        totalJumps = stats.NumJumps;
        numSpaces = stats.NumSpaces;
        pieceComplexity = stats.PieceComplexity;
    }
}