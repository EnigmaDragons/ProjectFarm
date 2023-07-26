using System.Linq;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class HeroAnimalSelectionController : OnMessage<HeroAnimalSelected>
{
    [SerializeField] private AllAnimals allAnimals;
    [SerializeField] private GameObject[] animals;
    [SerializeField] private CurrentHeroAnimal currentAnimal;

    private AudioSource _audioSource;
    
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        SetAnimal(currentAnimal.Current);
    }
    
    protected override void Execute(HeroAnimalSelected msg)
    {
        Log.SInfo(LogScopes.GameFlow, $"Selected Animal {msg.Selected}");
        SetAnimal(msg.Selected);

        var animalData = allAnimals.Animals.FirstOrDefault(a => a.Animal == msg.Selected);
        if (msg.IsConfirmed && _audioSource != null && animalData != null)
        {
            var sound = animalData.Sounds.Random();
            _audioSource.PlayOneShot(sound.clip, sound.volume);
        }
    }

    private void SetAnimal(HeroAnimal hero)
    {
        var selectedAnimalIndex = ((int)hero) - 1;
        for (var i = 0; i < animals.Length; i++)
            animals[i].SetActive(i == selectedAnimalIndex);
    }
}
