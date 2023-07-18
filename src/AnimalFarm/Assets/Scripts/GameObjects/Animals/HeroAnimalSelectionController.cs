using UnityEngine;

public class HeroAnimalSelectionController : OnMessage<HeroAnimalSelected>
{
    [SerializeField] private GameObject[] animals;
    
    protected override void Execute(HeroAnimalSelected msg)
    {
        Log.SInfo(LogScopes.GameFlow, $"Selected Animal {msg.Selected}");
        var selectedAnimalIndex = ((int)msg.Selected) - 1;
        for (var i = 0; i < animals.Length; i++)
            animals[i].SetActive(i == selectedAnimalIndex);
    }
}
