using System.Linq;
using UnityEngine;

public class CurrentAnimalSoundController : OnMessage<PlayCurrentAnimalSound, AnimalAudioSourceChanged>
{
    [SerializeField] private CurrentHeroAnimal animal;
    [SerializeField] private AllAnimals allAnimals;

    private AudioSource _audioSource;
    
    protected override void Execute(PlayCurrentAnimalSound msg)
    {
        if (_audioSource == null) 
            UpdateAudioSource();
        if (_audioSource != null)
        {
            var sound = allAnimals.Get(animal.Current).Sounds.Random();
            _audioSource.PlayOneShot(sound.clip, sound.volume);
        }
    }

    protected override void Execute(AnimalAudioSourceChanged msg)
    {
        _audioSource = msg.AudioSource;
    }

    private void UpdateAudioSource()
    {
        _audioSource = GetComponentsInChildren<AudioSource>().FirstOrDefault(x => x.gameObject.activeInHierarchy);
    }
}
