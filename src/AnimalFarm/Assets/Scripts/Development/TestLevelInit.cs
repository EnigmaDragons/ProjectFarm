using UnityEngine;

public class TestLevelInit : MonoBehaviour
{
    [SerializeField] private GameState game;
    [SerializeField] private CurrentLevel currentLevel; 
    [SerializeField] private GameLevel newLevel;
    [SerializeField] private string levelAssetFileName;

    private GameLevel level;
    
    public void Update()
    {
        if (!string.IsNullOrEmpty(levelAssetFileName))
        {
            var levelMap = JsonFileStored<LevelMapWithAnalysis>.Load(levelAssetFileName);

            return;
        }
        
        if (newLevel == null || newLevel == level) return;

        level = newLevel;
        currentLevel.SelectLevel(newLevel, 1, 1);
        game.InitLevel();
    }
}