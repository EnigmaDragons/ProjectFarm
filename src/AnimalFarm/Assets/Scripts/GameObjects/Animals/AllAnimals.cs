using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class AllAnimals : ScriptableObject
{
    [SerializeField] private HeroAnimalData[] animals;
    
    public HeroAnimalData[] Animals => animals;

    public HeroAnimalData Get(HeroAnimal animal)
    {
        var animalData = animals.FirstOrDefault(a => a.Animal == animal);
        if (animalData == null)
        {
            Log.Error($"Could not find animal {animal}");
            return animals[0];
        }
        return animalData;
    }
}
