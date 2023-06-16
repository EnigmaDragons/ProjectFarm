using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameDialogue : MonoBehaviour
{
    [SerializeField] private CurrentDialogue currentDialogue;
    [SerializeField] private TextMeshProUGUI name;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Navigator navigator;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button alternateContinueButton;
    [SerializeField] private Button skipButton;
    [SerializeField] private BoolVariable IsLevelStart;
    [SerializeField] private BoolReference OnlyStory;
    [SerializeField] private DialogueLine BetweenLevelDialogue;
    [SerializeField] private PlayerSurvey playerSurvey;
    [SerializeField] private GameObject displayParent;
    [SerializeField] private DialogueCharacterDisplay characterPrefab;
    [SerializeField] private float inputBufferSeconds;

    private readonly List<DialogueCharacterDisplay> _characters = new List<DialogueCharacterDisplay>();
    private Dialogue _currentDialogue;
    private int _nextIndex = 0;
    private float _inputBufferRemaining;

    private void Awake()
    {
        continueButton.onClick.AddListener(Continue);
        alternateContinueButton.onClick.AddListener(Continue);
        skipButton.onClick.AddListener(Skip);
    }

    private void Start()
    {
        var dialogue = IsLevelStart.Value
            ? currentDialogue.Dialogue.Value.Intro 
            : currentDialogue.Dialogue.Value.Outro;
        if (dialogue.Lines.Length == 0)
            Finish();
        else
        {
            _currentDialogue = dialogue;
            InitStartingCharacters();
            _nextIndex = 0;
            Continue();
        }
    }

    private void Update() => _inputBufferRemaining = Math.Max(0, _inputBufferRemaining -= Time.deltaTime);

    private void Finish()
    {
        if (IsLevelStart.Value && OnlyStory.Value)
        {
            IsLevelStart.Value = false;
            navigator.NavigateToDialogue();
        }
        else if (OnlyStory.Value)
            navigator.NavigateToArchive();
        else if (IsLevelStart.Value) 
            navigator.NavigateToGameScene();
        else if (playerSurvey.HasSurvey)
            navigator.NavigateToSurvey();
        else
            navigator.NavigateToRewards();
    }

    public void Continue()
    {
        if (_inputBufferRemaining > 0)
            return;
        if (_nextIndex == _currentDialogue.Lines.Length && OnlyStory.Value && IsLevelStart.Value)
        {
            text.text = BetweenLevelDialogue.Text;
            name.text = BetweenLevelDialogue.Character.Name;
            _characters.ForEach(x => x.SetFocus(false));
            _nextIndex++;
            _inputBufferRemaining = inputBufferSeconds;
        }
        else if (_nextIndex >= _currentDialogue.Lines.Length)
            Finish();
        else if (_currentDialogue.Lines[_nextIndex].Type == DialogueLineType.StatementOnly)
        {
            var line = _currentDialogue.Lines[_nextIndex];
            text.text = line.Text;
            name.text = line.Character.Name;
            _characters.ForEach(x => x.SetFocus(x.Character.Name == line.Character.Name));
            _nextIndex++;
            _inputBufferRemaining = inputBufferSeconds;
            Message.Publish(new DialogueAdvanced());
        }
        else
        {
            var line = _currentDialogue.Lines[_nextIndex];
            if (line.Type == DialogueLineType.CharacterExits)
                RemoveCharacter(line.Character);
            else if (line.Type == DialogueLineType.CharacterEnter)
                AddCharacter(line.Character);
            _nextIndex++;
            _inputBufferRemaining = 0;
            Continue();
        }
    }

    public void Skip()
    {
        Finish();
    }

    private void InitStartingCharacters()
    {
        for (var i = 0; i < _currentDialogue.StartingCharacters.Length; i++)
        {
            var display = Instantiate(characterPrefab, displayParent.transform);
            display.Init(_currentDialogue.StartingCharacters[i],  _characters.Count == 0);
            _characters.Add(display);
        }
        SetCharacterLocations(true);
    }

    private void AddCharacter(Character character)
    {
        var display = Instantiate(characterPrefab, displayParent.transform);
        display.Init(character, _characters.Count == 0);
        _characters.Add(display);
        SetCharacterLocations(false);
    }

    private void RemoveCharacter(Character character)
    {
        var display = _characters.First(x => x.Character.Name == character.Name);
        display.Leave();
        _characters.Remove(display);
        SetCharacterLocations(false);
    }

    private void SetCharacterLocations(bool isTeleporting)
    {
        for (var i = 0; i < _characters.Count; i++)
        {
            if (i == 0 && _characters.Count > 1)
                _characters[i].GoTo(DialogueDirection.Left, isTeleporting);
            else if (i != 0 && i + 1 == _currentDialogue.StartingCharacters.Length)
                _characters[i].GoTo(DialogueDirection.Right, isTeleporting);
            else
                _characters[i].GoTo(DialogueDirection.Center, isTeleporting);
        }
    }
}

