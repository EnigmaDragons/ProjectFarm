using NUnit.Framework;

public class GenTests
{
    [Test]
    public void GenerateOneLevel()
    {
        GenPipeline.CreateLevels(1);
    }
    
    [Test]
    public void LevelLimitTester()
    {
        GenPipeline.CreateOne(new LevelGenV1Params
        {
            MinMoves = 28,
            MaxMoves = 28,
            MaxConsecutiveMisses = 24, 
            SkipAnalysis = true,
        });
    }
}
