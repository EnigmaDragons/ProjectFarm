using UnityEngine;

public class HideDuringHeroPath : OnMessage<HeroPathBegun>
{
    [SerializeField] private GameObject[] hideTargets;
    
    protected override void Execute(HeroPathBegun msg) => hideTargets.ForEach(h => h.SetActive(false));
}
