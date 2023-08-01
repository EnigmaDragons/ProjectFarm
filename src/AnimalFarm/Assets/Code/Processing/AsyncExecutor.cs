using System;
using System.Collections;
using UnityEngine;

public class AsyncExecutor : MonoBehaviour
{
    private static AsyncExecutor instance;

    private static AsyncExecutor Instance
    {
        get
        {
            if (instance == null)
            {
                var o = new GameObject("AsyncExecutor");
                instance = o.AddComponent<AsyncExecutor>();
                DontDestroyOnLoad(o);
            }
            return instance;
        }
    }
    
    private void Awake()
    {
        instance = this;
    }
    
    public static void PublishMessageAfterDelay<T>(float delaySeconds, T message)
    {
        Instance.StartCoroutine(PublishMessageAfterDelayCoroutine(delaySeconds, message));
    }

    private static IEnumerator PublishMessageAfterDelayCoroutine<T>(float delaySeconds, T message)
    {
        yield return new WaitForSeconds(delaySeconds);
        Message.Publish(message);
    }

    public static void ExecuteAfterDelay(float delaySeconds, Action action)
    {
        Instance.StartCoroutine(ExecuteAfterDelayCoroutine(delaySeconds, action));
    }

    private static IEnumerator ExecuteAfterDelayCoroutine(float delaySeconds, Action action)
    {
        yield return new WaitForSeconds(delaySeconds);
        action();
    }
}
