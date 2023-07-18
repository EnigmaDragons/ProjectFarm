using UnityEngine;

[CreateAssetMenu]
public class CurrentHeroAnimal : ScriptableObject
{
    [SerializeField] private HeroAnimal previous = HeroAnimal.NotSelected;
    [SerializeField] private HeroAnimal selected = HeroAnimal.NotSelected;

    public HeroAnimal Previous => previous;
    public HeroAnimal Current => selected;
    
    public void Init() => selected = HeroAnimal.NotSelected;
    
    public void Select(HeroAnimal animal)
    {
        previous = selected;
        selected = animal;
        Message.Publish(new HeroAnimalSelected(animal, isConfirmed: false));
    }

    public void Confirm()
    {
        Message.Publish(new HeroAnimalSelected(Current, isConfirmed: true));
    }
}
