using UnityEngine;

[CreateAssetMenu]
public class HeroAnimalData : ScriptableObject
{
    [SerializeField] private HeroAnimal animal;
    [SerializeField] private Sprite image;
    
    public HeroAnimal Animal => animal;
    public Sprite Image => image;
}
