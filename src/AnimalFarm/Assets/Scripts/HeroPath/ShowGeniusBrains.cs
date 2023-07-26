using System.Collections;
using UnityEngine;

public class ShowGeniusBrains : OnMessage<HeroPathBegun>
{
    [SerializeField] private GameObject parent;
    [SerializeField] private GameObject brainPrototype;
    [SerializeField] private FloatReference delayBetweenBrains = new FloatReference(0.5f);
    
    protected override void Execute(HeroPathBegun msg) => StartCoroutine(Go());

    private IEnumerator Go()
    {
        for (var i = 0; i < 3; i++)
        {
            Instantiate(brainPrototype, parent.transform);
            yield return new WaitForSeconds(delayBetweenBrains);
        }
    }
}
