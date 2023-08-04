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
    [SerializeField] private bool isGenius;
    
    public bool IsGenius => isGenius;
    public CurrentLevel CurrentLevel => currentLevel;

    [Obsolete]
    public void InitLevel()
    {
        isGenius = false;
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
        Debug.Log($"Game State Init - Is Reset {isReset}");
        if (!isReset)
            currentAnimal.Init();
        isGenius = false;
        currentZone.Init(currentLevel.ZoneNumber);
        currentLevelStars.Reset();
        currentMoveCounter.Reset();
        currentLevelMap.InitLevel(CurrentLevel.ActiveLevelName);
        currentPiece.Deselect();
        currentLevel.Init();
    }
    
    public void FinishInitGeneratedLevelMap(bool isReset)
    {
        currentLevelMap.RegisterHeroPath(CurrentLevel.ActiveMap.HeroPath);
        currentLevelMap.FinalizeInitialCounters();
        hasResetLevel.Value = false;
        Message.Publish(new LevelReset());
        if (isReset)
            AsyncExecutor.PublishMessageAfterDelay(0.01f, new PieceSelected(currentLevelMap.Hero));
    }
    
    public void SetIsGenius()
    {
        isGenius = true;
    }
}
