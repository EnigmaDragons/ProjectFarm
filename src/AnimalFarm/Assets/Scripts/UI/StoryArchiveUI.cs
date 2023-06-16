using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoryArchiveUI : MonoBehaviour
{
    [SerializeField] private StoryArchiveButton[] buttons;
    [SerializeField] private Button previous;
    [SerializeField] private Button next;
    [SerializeField] private TextMeshProUGUI num;
    [SerializeField] private SaveStorage saveStorage;
    [SerializeField] private BoolReference developmentToolsActive;
    [SerializeField] private BoolVariable isStoryOnly;
    [SerializeField] private BoolVariable isLevelStart;
    [SerializeField] private CurrentDialogue dialogue;
    [SerializeField] private Navigator navigator;
    [SerializeField] private CurrentZone zone;

    private List<List<StoryChoice>> unlockedStories;
    private int _index;

    private void Start()
    {
        previous.onClick.AddListener(Previous);
        next.onClick.AddListener(Next);
        var stories = zone.Campaign.Value
            .SelectMany((xZone, zoneI) => Enumerable
                .Range(0, developmentToolsActive ? 99 : xZone.Value.Count(level => saveStorage.GetStars(level) > 0))
                .Where(i => i < xZone.Story.Length)
                .Select(i => new StoryChoice(xZone.Story[i], zoneI, i + 1)))
            .ToList();
        unlockedStories = new List<List<StoryChoice>>();
        for (var i = 0; i < stories.Count; i += buttons.Length)
            unlockedStories.Add(stories.Skip(i).Take(buttons.Length).ToList());
        _index = zone.ZoneIndex * 2;
        isStoryOnly.Value = false;
        UpdateButtons();
    }

    private void Previous()
    {
        _index--;
        UpdateButtons();
    }

    private void Next()
    {
        _index++;
        UpdateButtons();
    }

    private void UpdateButtons()
    {
        previous.gameObject.SetActive(_index != 0);
        next.gameObject.SetActive(_index != unlockedStories.Count - 1);
        num.text = (_index + 1).ToString();
        for (var i = 0; i < buttons.Length; i++)
        {
            if (i >= unlockedStories[_index].Count)
                buttons[i].Init("", () => { });
            else
            {
                var closuredI = i;
                buttons[i].Init(unlockedStories[_index][i].Name, () =>
                {
                    isLevelStart.Value = true;
                    dialogue.Set(unlockedStories[_index][closuredI].Story);
                    isStoryOnly.Value = true;
                    navigator.NavigateToDialogue();
                });
            }
        }
    }
}

public class StoryChoice
{
    public readonly ConjoinedDialogues Story;
    public readonly string Name;

    public StoryChoice(ConjoinedDialogues story, int zone, int num)
    {
        Story = story;
        Name = $"{zone + 1}-{num} {Story.Intro.DialogueName}";
    }
}
