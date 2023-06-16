using UnityEngine;
using UnityEngine.UI;

public class NewGameButtons : MonoBehaviour
{
    [SerializeField] private Navigator navigator;
    [SerializeField] private Toggle storyMode;
    [SerializeField] private Toggle onlyPuzzles;
    [SerializeField] private Toggle male;
    [SerializeField] private Toggle female;
    [SerializeField] private Button start;
    [SerializeField] private BoolVariable autoSkip;
    [SerializeField] private BoolVariable useFemale;
    [SerializeField] private SaveStorage saveStorage;

    private bool _autoSkip = false;
    private bool _useFemale = false;

    private void Awake()
    {
        storyMode.onValueChanged.AddListener(_ => SelectStoryMode());
        onlyPuzzles.onValueChanged.AddListener(_ => SelectOnlyPuzzles());
        male.onValueChanged.AddListener(_ => SelectMale());
        female.onValueChanged.AddListener(_ => SelectFemale());
        start.onClick.AddListener(StartGame);
        OnEnable();
    }

    private void OnEnable()
    {
        _autoSkip = false;
        _useFemale = useFemale.Value;
        storyMode.SetIsOnWithoutNotify(!_autoSkip);
        onlyPuzzles.SetIsOnWithoutNotify(_autoSkip);
        male.SetIsOnWithoutNotify(!_useFemale);
        female.SetIsOnWithoutNotify(_useFemale);
    }

    private void SelectStoryMode()
    {
        _autoSkip = false;
        storyMode.SetIsOnWithoutNotify(true);
        onlyPuzzles.SetIsOnWithoutNotify(false);
    }

    private void SelectOnlyPuzzles()
    {
        _autoSkip = true;
        storyMode.SetIsOnWithoutNotify(false);
        onlyPuzzles.SetIsOnWithoutNotify(true);
    }

    private void SelectMale()
    {
        _useFemale = false;
        male.SetIsOnWithoutNotify(true);
        female.SetIsOnWithoutNotify(false);
    }

    private void SelectFemale()
    {
        _useFemale = true;
        male.SetIsOnWithoutNotify(false);
        female.SetIsOnWithoutNotify(true);
    }

    public void StartGame()
    {
        saveStorage.Init();
        saveStorage.StartNewGame();
        saveStorage.SetUseFemale(_useFemale);
        saveStorage.SetAutoSkipStory(_autoSkip);
        useFemale.Value = _useFemale;
        autoSkip.Value = _autoSkip;
        navigator.NavigateToLevelSelect();
    }
}
