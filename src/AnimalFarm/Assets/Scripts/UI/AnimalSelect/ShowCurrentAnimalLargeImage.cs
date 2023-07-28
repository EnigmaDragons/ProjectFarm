using UnityEngine;
using UnityEngine.UI;

public class ShowCurrentAnimalLargeImage : MonoBehaviour
{
    [SerializeField] private CurrentHeroAnimal current;
    [SerializeField] private AllAnimals animals;
    [SerializeField] private Image image;

    private void OnEnable()
    {
        image.sprite = animals.Get(current.Current).Image1024;
    }
}
