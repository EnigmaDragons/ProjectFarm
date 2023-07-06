using System;
using System.IO;
using System.Linq;
using UnityEngine;

public static class GenPipeline
{
    public static LevelMap[] CreateLevels(int n)
    {
        return Enumerable.Range(0, n).Select(_ => CreateOne()).ToArray();
    }
    
    public static LevelMap CreateOne()
    {
        var level = Generate();
        var analysis = Analyze(level);
        Persist(level, analysis);
        return level;
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

        var filename = $".//Assets//Data//GenLevels//{guid}.json";
        using var writer = new StreamWriter(filename);
        writer.Write(JsonUtility.ToJson(item));
        Debug.Log($"Persisted: {filename}");
    }
}
