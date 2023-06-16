using System;
using UnityEngine;

public class BasicPuzzleComplexityStats
{
    public string LevelName { get; }
    public int NumSpaces { get; }
    public int NumJumps { get; }
    public int PieceComplexity { get; }
    public int Score => NumJumps * JumpsWeight + PieceComplexity * PieceComplexityWeight;
    public int Difficulty => OneToEleven((int)Mathf.Ceil((Score - 54f) / (PieceComplexityWeight + JumpsWeight * 7.5f)));

    private int OneToEleven(int val) => Clamp(val, 1, 11);
    private int Clamp(int val, int min, int max) => Math.Max(min, Math.Min(max, val)); 
    private int PieceComplexityWeight = 2;
    private int JumpsWeight = 4;
    
    public BasicPuzzleComplexityStats(string name, int numSpaces, int numJumps, int pieceComplexity)
    {
        LevelName = name;
        NumSpaces = numSpaces;
        NumJumps = numJumps;
        PieceComplexity = pieceComplexity;
    }
    
    public override string ToString() => $"{LevelName}, Difficulty {Difficulty}, {Score}, - , {NumSpaces}, {NumJumps}, {PieceComplexity}";
}