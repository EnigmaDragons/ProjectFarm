using UnityEngine;

public class GameState : ScriptableObject
{
    [SerializeField] private CurrentLevelMap currentLevelMap;
    [SerializeField] private CurrentSelectedPiece currentPiece;
    [SerializeField] private CurrentLevelStars currentLevelStars;
    [SerializeField] private CurrentMoveCounter currentMoveCounter;
    [SerializeField] private CurrentLevel currentLevel;
    [SerializeField] private CurrentZone currentZone;
    [SerializeField] private BoolVariable hasResetLevel;

    public CurrentLevel CurrentLevel => currentLevel;

    public void InitLevel()
    {
        currentZone.Init(currentLevel.ZoneNumber);
        currentLevelStars.Reset();
        currentMoveCounter.Reset();
        currentLevelMap.InitLevel(CurrentLevel.ActiveLevel.name);
        currentPiece.Deselect();
        currentLevel.Init();
        hasResetLevel.Value = false;
        Message.Publish(new LevelReset());
    }
}
