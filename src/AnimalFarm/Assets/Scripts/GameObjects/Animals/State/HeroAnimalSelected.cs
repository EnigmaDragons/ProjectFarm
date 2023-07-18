
public class HeroAnimalSelected
{
    public HeroAnimal Selected { get; }
    public bool IsConfirmed { get; }

    public HeroAnimalSelected(HeroAnimal selected, bool isConfirmed)
    {
        Selected = selected;
        IsConfirmed = isConfirmed;
    }
}
