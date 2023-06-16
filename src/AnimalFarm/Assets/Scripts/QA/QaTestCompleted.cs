using System.Collections.Generic;

public sealed class QaTestCompleted
{
    public bool Passed => Issues.Count < 1;
    public List<string> Issues { get; }

    public QaTestCompleted(List<string> issues)
    {
        Issues = issues;
    }
}
