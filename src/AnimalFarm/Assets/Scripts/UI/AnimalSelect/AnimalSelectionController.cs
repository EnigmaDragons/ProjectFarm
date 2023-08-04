using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnimalSelectionController : OnMessage<LevelReset>
{
    [SerializeField] private LockBoolVariable gameInputActive;
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private Image animalImage;
    [SerializeField] private Image animalBackdrop;
    [SerializeField] private TextMeshProUGUI animalNumberLabel;
    [SerializeField] private CurrentHeroAnimal currentAnimal;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;
    [SerializeField] private Button confirmButton;
    [SerializeField] private AllAnimals allAnimals;

    private bool _isInitialized;
    private IndexSelector<HeroAnimalData> _animalSelector;

    private void Awake()
    {
        InitIfNeeded();
    }

    private void InitIfNeeded()
    {
        if (_isInitialized)
            return;

        _isInitialized = true;
        uiPanel.SetActive(false);
        _animalSelector = new IndexSelector<HeroAnimalData>(allAnimals.Animals.OrderBy(x => (int)x.Animal).ToArray());
        InitAnimals();
        nextButton.onClick.AddListener(MoveNext);
        prevButton.onClick.AddListener(MovePrevious);
        confirmButton.onClick.AddListener(Confirm);
    }

    private void InitAnimals()
    {
        if (currentAnimal.Previous != HeroAnimal.NotSelected)
            while (_animalSelector.Current.Animal != currentAnimal.Previous)
                _animalSelector.MoveNext();
        Refresh();
    }

    public void BeginSelection()
    {
        InitIfNeeded();
        uiPanel.SetActive(true);
        gameInputActive.Lock(gameObject);
        InitAnimals();
        Debug.Log("Animal Selection Begun");
        Message.Publish(new AnimalSelectionBegun());
    }
    
    protected override void Execute(LevelReset msg)
    {
        Debug.Log("Level Reset");
        Debug.Log($"Current Animal = {currentAnimal.Current}");
        if (currentAnimal.Current == HeroAnimal.NotSelected)
        {
            BeginSelection();
        }
        else
        {
            uiPanel.SetActive(false);
            Debug.Log("Animal Selection Finished/Skipped");
            Message.Publish(new AnimalSelectionFinished());
        }
    }

    private void Confirm()
    {
        uiPanel.SetActive(false);
        gameInputActive.Unlock(gameObject);
        currentAnimal.Confirm();
        Message.Publish(new AnimalSelectionFinished());
    }
    
    private void MoveNext()
    {
        _animalSelector.MoveNext();
        Refresh();
    }

    private void MovePrevious()
    {
        _animalSelector.MovePrevious();
        Refresh();
    }

    // TODO: Animate
    private void Refresh()
    {
        var c = _animalSelector.Current;
        currentAnimal.Select(c.Animal);
        animalImage.sprite = c.Image;
        animalBackdrop.sprite = c.Image;
        animalNumberLabel.text = ((int)c.Animal).ToString();
    }
}
