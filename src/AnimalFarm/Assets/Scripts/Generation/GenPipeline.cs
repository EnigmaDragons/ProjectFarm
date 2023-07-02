using System;
using System.IO;
using UnityEngine;

public static class GenPipeline
{
    public static void CreateLevels(int n)
    {
        for(var i = 0; i < n; i++)
            CreateOne();
    }
    
    public static void CreateOne()
    {
        var level = Generate();
        var analysis = Analyze(level);
        Persist(level, analysis);
    }
    
    public static LevelMap Generate()
    {
        return LevelGenV1.Generate(new LevelGenV1Params
        {
            MinMoves = 1,
            MaxMoves = 3,
        });
    }

    public static GenAnalysisResult Analyze(LevelMap m)
    {
        return GenAnalyzer.Analyze(m);
    }

    public static void Persist(LevelMap level, GenAnalysisResult analysis)
    {
        var guid = Guid.NewGuid().ToString();
        var levelMapString = new TokenizedLevelMap(level).ToString();
        var item = new LevelMapWithAnalysis
        {
            Analysis = analysis,
            LevelMapString = levelMapString,
        };
        
        using var writer = new StreamWriter($".//Assets//Data//GenLevels//{guid}.map");
        writer.Write(JsonUtility.ToJson(item));
    }
}
