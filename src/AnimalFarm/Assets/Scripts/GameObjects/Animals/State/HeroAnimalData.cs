using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class HeroAnimalData : ScriptableObject
{
    [SerializeField] private HeroAnimal animal;
    [SerializeField, FormerlySerializedAs("image")] private Sprite image256;
    [SerializeField] private Sprite image1024;
    [SerializeField] private AudioClipWithVolume[] sounds;
    
    public HeroAnimal Animal => animal;
    public Sprite Image => image256;
    public Sprite Image1024 => image1024;
    public AudioClipWithVolume[] Sounds => sounds;
}
