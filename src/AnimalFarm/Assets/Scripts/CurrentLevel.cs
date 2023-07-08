using UnityEngine;

public class CurrentLevel : ScriptableObject
{
    [SerializeField] private LevelMap anonMap;
    [SerializeField] private GameLevel selectedLevel;
    [SerializeField] private GameObject activeLevelPrefab;
    [SerializeField] private int currentZoneNum;
    [SerializeField] private int currentLevelNum;

    private Transform anonMapTransform;
    
    public string ActiveLevelName
    {
        get
        {
            if (anonMap != null)
                return "Generated Map";
            if (selectedLevel != null)
                return selectedLevel.Name;
            return "None";
        }
    }

    public LevelMap ActiveMap => anonMap;
    public Transform ActiveMapTransform => anonMapTransform;
    
    public GameLevel ActiveLevel => selectedLevel;

    public Transform Transform => ActiveMapTransform != null ? ActiveMapTransform : activeLevelPrefab.transform;
    public int ZoneNumber => currentZoneNum;
    public int LevelNumber => currentLevelNum;
    
    public void SelectLevel(GameLevel level, int zoneNum, int levelNum)
    {
        Log.SInfo(LogScopes.GameFlow, $"Selected Z{zoneNum}-{levelNum} level {level.Name}");
        selectedLevel = level;
        currentZoneNum = zoneNum;
        currentLevelNum = levelNum;
        anonMap = null;
    }

    public void UseGenMap(LevelMap levelMap, Transform obj)
    {
        anonMap = levelMap;
        selectedLevel = null;
        anonMapTransform = obj;
        Log.SInfo(LogScopes.GameFlow, $"Use Generated Map");
    }
    
    public void Init()
    {
        Log.SInfo(LogScopes.GameFlow, $"Initialized Level {ActiveLevelName}");
        Clear();
        if (selectedLevel != null)
            activeLevelPrefab = Instantiate(selectedLevel.Prefab);
    }

    public void Clear()
    {
        if (activeLevelPrefab != null)
            DestroyImmediate(activeLevelPrefab);
    }
}