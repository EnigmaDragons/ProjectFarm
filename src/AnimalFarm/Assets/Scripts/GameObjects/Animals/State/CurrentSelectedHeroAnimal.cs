using UnityEngine;

[CreateAssetMenu]
public class CurrentSelectedHeroAnimal : ScriptableObject
{
    [SerializeField] private HeroAnimal selected = HeroAnimal.NotSelected;

    public HeroAnimal Current => selected;
    
    public void Init() => selected = HeroAnimal.NotSelected;
    
    public void Select(HeroAnimal animal)
    {
        selected = animal;
        Message.Publish(new HeroAnimalSelected(animal));
    }
}
