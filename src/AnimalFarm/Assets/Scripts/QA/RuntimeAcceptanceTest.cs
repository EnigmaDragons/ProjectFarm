using System.Collections.Generic;
using UnityEngine;

public abstract class RuntimeAcceptanceTest : MonoBehaviour
{
    [SerializeField] private bool runOnAwake;
    [SerializeField] private bool runOnStart;
    
#if UNITY_EDITOR
    void Awake()
    {
        if (runOnAwake)
            RunTest();
    }

    void Start()
    {
        if (runOnStart)
            RunTest();
    }

    public void RunTest()
    {
        Message.Publish(new QaTestStarted());
        var issues = GetAllIssues();
        Message.Publish(new QaTestCompleted(issues));
    }
#endif
    
    protected abstract List<string> GetAllIssues();
}
