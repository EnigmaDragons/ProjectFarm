using UnityEngine;

[CreateAssetMenu]
public class AllAnimals : ScriptableObject
{
    [SerializeField] private HeroAnimalData[] animals;
    
    public HeroAnimalData[] Animals => animals;
}
