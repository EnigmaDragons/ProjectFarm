using System;
using UnityEngine;

public class GameState : ScriptableObject
{
    [SerializeField] private CurrentLevelMap currentLevelMap;
    [SerializeField] private CurrentSelectedPiece currentPiece;
    [SerializeField] private CurrentLevelStars currentLevelStars;
    [SerializeField] private CurrentMoveCounter currentMoveCounter;
    [SerializeField] private CurrentLevel currentLevel;
    [SerializeField] private CurrentZone currentZone;
    [SerializeField] private CurrentHeroAnimal currentAnimal;
    [SerializeField] private BoolVariable hasResetLevel;
    
    public CurrentLevel CurrentLevel => currentLevel;

    [Obsolete]
    public void InitLevel()
    {
        currentZone.Init(currentLevel.ZoneNumber);
        currentLevelStars.Reset();
        currentMoveCounter.Reset();
        currentLevelMap.InitLevel(CurrentLevel.ActiveLevel.name);
        currentPiece.Deselect();
        currentLevel.Init();
        currentAnimal.Init();
        hasResetLevel.Value = false;
        Message.Publish(new LevelReset());
    }

    public void BeginInitGeneratedLevelMap(bool isReset)
    {
        currentZone.Init(currentLevel.ZoneNumber);
        currentLevelStars.Reset();
        currentMoveCounter.Reset();
        currentLevelMap.InitLevel(CurrentLevel.ActiveLevelName);
        currentPiece.Deselect();
        currentLevel.Init();
        if (!isReset)
            currentAnimal.Init();
    }
    
    public void FinishInitGeneratedLevelMap()
    {
        currentLevelMap.RegisterHeroPath(CurrentLevel.ActiveMap.HeroPath);
        currentLevelMap.FinalizeInitialCounters();
        hasResetLevel.Value = false;
        Message.Publish(new LevelReset());
    }
}
