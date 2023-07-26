using System.Collections;
using UnityEngine;

public sealed class OnLevelCompleteSpawnWithDelay : OnMessage<LevelCompleted>
{
    [SerializeField] private StarCounter counter;
    [SerializeField] private GameObject prototype;
    [SerializeField] private GameObject parent;
    [SerializeField] private FloatReference initialDelay = new FloatReference(1);
    [SerializeField] private FloatReference delayBetween = new FloatReference(0.4f);
    [SerializeField] private GameState gameState;
    [SerializeField] private GameObject geniusPrototype;

    protected override void Execute(LevelCompleted msg) => StartCoroutine(Go());
    
    // TODO: Optimize by pre-instantiating all stars and then just enabling them
    private IEnumerator Go()
    {
        yield return new WaitForSeconds(initialDelay);
        if (gameState.IsGenius)
        {
            for (var i = 0; i < 3; i++)
            {
                Instantiate(geniusPrototype, parent.transform);
                yield return new WaitForSeconds(delayBetween);
                // Extra delay because of cost of Instantiate
                if (i == 0)
                    yield return new WaitForSeconds(delayBetween);
            }
        }
        else
        {
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
}
