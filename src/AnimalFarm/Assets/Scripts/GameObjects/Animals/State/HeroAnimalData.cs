using UnityEngine;

[CreateAssetMenu]
public class HeroAnimalData : ScriptableObject
{
    [SerializeField] private HeroAnimal animal;
    [SerializeField] private Sprite image;
    [SerializeField] private AudioClipWithVolume[] sounds;
    
    public HeroAnimal Animal => animal;
    public Sprite Image => image;
    public AudioClipWithVolume[] Sounds => sounds;
}
