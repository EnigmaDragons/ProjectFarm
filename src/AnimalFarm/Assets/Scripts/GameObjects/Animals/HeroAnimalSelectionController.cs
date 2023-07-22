using System.Linq;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class HeroAnimalSelectionController : OnMessage<HeroAnimalSelected>
{
    [SerializeField] private AllAnimals allAnimals;
    [SerializeField] private GameObject[] animals;

    private AudioSource _audioSource;
    
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }
    
    protected override void Execute(HeroAnimalSelected msg)
    {
        Log.SInfo(LogScopes.GameFlow, $"Selected Animal {msg.Selected}");
        var selectedAnimalIndex = ((int)msg.Selected) - 1;
        for (var i = 0; i < animals.Length; i++)
            animals[i].SetActive(i == selectedAnimalIndex);
        
        var animalData = allAnimals.Animals.FirstOrDefault(a => a.Animal == msg.Selected);
        if (msg.IsConfirmed && _audioSource != null && animalData != null)
        {
            var sound = animalData.Sounds.Random();
            _audioSource.PlayOneShot(sound.clip, sound.volume);
        }
    }
}
