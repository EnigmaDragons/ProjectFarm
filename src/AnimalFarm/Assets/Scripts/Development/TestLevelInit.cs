using UnityEngine;

public class TestLevelInit : MonoBehaviour
{
    [SerializeField] private GameState game;
    [SerializeField] private CurrentLevel currentLevel; 
    [SerializeField] private GameLevel newLevel;

    private GameLevel level;
    
    public void Update()
    {
        if (newLevel == null || newLevel == level) return;

        level = newLevel;
        currentLevel.SelectLevel(newLevel, 1, 1);
        game.InitLevel();
    }
}