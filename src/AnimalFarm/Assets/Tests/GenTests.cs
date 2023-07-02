using NUnit.Framework;

public class GenTests
{
    [Test]
    public void GenerateOneLevel()
    {
        GenPipeline.CreateLevels(1);
    }
}
