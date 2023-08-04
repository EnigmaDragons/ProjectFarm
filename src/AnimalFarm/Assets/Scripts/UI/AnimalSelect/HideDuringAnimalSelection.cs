using UnityEngine;

public class HideDuringAnimalSelection : OnMessage<AnimalSelectionBegun, AnimalSelectionFinished>
{
    [SerializeField] private GameObject[] targets;
    [SerializeField] private GameObject[] showTargets;

    private void Awake() => SetForAnimalSelection();
    
    protected override void Execute(AnimalSelectionBegun msg) => SetForAnimalSelection();
    protected override void Execute(AnimalSelectionFinished msg) => SetForNonAnimalSelection();

    private void SetForAnimalSelection()
    {
        targets.ForEach(x => x.SetActive(false));
        showTargets.ForEach(x => x.SetActive(true));
    }

    private void SetForNonAnimalSelection()
    {
        targets.ForEach(x => x.SetActive(true));
        showTargets.ForEach(x => x.SetActive(false));
    }
}
