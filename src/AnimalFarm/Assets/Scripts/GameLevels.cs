using System;
using System.Collections.Generic;
using E7.Introloop;
using UnityEngine;

[CreateAssetMenu]
public sealed class GameLevels : ScriptableObject
{
    [SerializeField] private GameLevel[] value;
    [SerializeField] private ConjoinedDialogues[] story; 
    [SerializeField] private IntReference starsRequired;
    [SerializeField] private UnityDateTimeOffset minDateRequired = DateTimeOffset.MinValue.AddDays(2); 
    [SerializeField] private string name;
    [SerializeField] private string description;
    [SerializeField] private Sprite logo;
    [SerializeField] private Sprite logoTiled;
    [SerializeField] private Color logoColor;
    [SerializeField] private Color backgroundColor;
    [SerializeField] private IntroloopAudio musicTheme;
    [SerializeField] private SaveStorage saveStorage;
    [SerializeField] private GameLevel tutorial;
    [SerializeField] private int[] progression;

    public GameLevel[] Value => value;
    public ConjoinedDialogues[] Story => story;
    public int StarsRequired => starsRequired;
    public DateTimeOffset MinDateRequired => minDateRequired;
    public string Name => name;
    public string Description => description;
    public Sprite Logo => logo;
    public Sprite LogoTiled => logoTiled;
    public Color LogoColor => logoColor;
    public Color BackgroundColor => backgroundColor;
    public IntroloopAudio MusicTheme => musicTheme;
    public Maybe<GameLevel> Tutorial => tutorial;
    public int[] Progression => progression;

    public Maybe<ConjoinedDialogues> CurrentStory()
    {
        var index = saveStorage.GetLevelsCompletedInZone(this);
        return index >= story.Length ? new Maybe<ConjoinedDialogues>() : story[index];
    }
}
