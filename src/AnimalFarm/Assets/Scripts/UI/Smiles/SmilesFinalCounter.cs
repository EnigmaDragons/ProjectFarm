using System.Collections;
using UnityEngine;

public sealed class SmilesFinalCounter : OnMessage<ShowFinalSmiles>
{
    [SerializeField] private GameObject[] toEnable;
    [SerializeField] private GameObject prototype;
    [SerializeField] private GameObject parent;
    [SerializeField] private FloatReference initialDelay = new FloatReference(1);
    [SerializeField] private FloatReference delayBetween = new FloatReference(0.4f);

    protected override void Execute(ShowFinalSmiles msg) => StartCoroutine(Go(msg.Number));
    
    // TODO: Optimize by pre-instantiating all stars and then just enabling them
    private IEnumerator Go(int number)
    {
        toEnable.ForEach(x => x.gameObject.SetActive(true));
        yield return new WaitForSeconds(initialDelay);
        for (var i = 0; i < number; i++)
        {
            Instantiate(prototype, parent.transform);
            yield return new WaitForSeconds(delayBetween);
            // Extra delay because of cost of Instantiate
            if (i == 0)
                yield return new WaitForSeconds(delayBetween);
        }
    }
}
