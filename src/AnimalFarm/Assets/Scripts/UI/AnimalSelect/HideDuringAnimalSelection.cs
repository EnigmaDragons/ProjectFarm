using UnityEngine;

public class HideDuringAnimalSelection : OnMessage<AnimalSelectionBegun, AnimalSelectionFinished>
{
    [SerializeField] private GameObject[] targets;
    [SerializeField] private GameObject[] showTargets;
    
    protected override void Execute(AnimalSelectionBegun msg)
    {
        targets.ForEach(x => x.SetActive(false));
        showTargets.ForEach(x => x.SetActive(true));
    }

    protected override void Execute(AnimalSelectionFinished msg)
    {
        targets.ForEach(x => x.SetActive(true)); 
        showTargets.ForEach(x => x.SetActive(false));
    }
}
