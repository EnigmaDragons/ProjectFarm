using System.Collections;
using UnityEngine;

public sealed class OnLevelCompleteSpawnWithDelay : OnMessage<LevelCompleted>
{
    [SerializeField] private StarCounter counter;
    [SerializeField] private GameObject prototype;
    [SerializeField] private GameObject parent;
    [SerializeField] private FloatReference initialDelay = new FloatReference(1);
    [SerializeField] private FloatReference delayBetween = new FloatReference(0.4f);

    protected override void Execute(LevelCompleted msg) => StartCoroutine(Go());
    
    private IEnumerator Go()
    {
        yield return new WaitForSeconds(initialDelay);
        for (var i = 0; i < counter.NumStars; i++)
        {
            Instantiate(prototype, parent.transform);
            yield return new WaitForSeconds(delayBetween);
            // Extra delay because of cost of Instantiate
            if (i == 0)
                yield return new WaitForSeconds(delayBetween);
        }
    }
}
