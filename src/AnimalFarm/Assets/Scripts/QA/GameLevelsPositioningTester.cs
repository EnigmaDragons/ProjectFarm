using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class GameLevelsPositioningTester : RuntimeAcceptanceTest
{
    [SerializeField] private CurrentLevelMap map;
    [SerializeField] private CurrentLevel current;
    
    protected override List<string> GetAllIssues()
    {
        var issues = new List<string>();
        var levels = new HashSet<string>();
#if UNITY_EDITOR        
        var gameLevels = UnityResourceUtils.FindAssetsByType<GameLevels>();
        gameLevels.SelectMany(zone => zone.Value).ForEach(level =>
        {
            var n = $"{level.name}";
            levels.Add(level.GetInstanceID().ToString());
            map.InitLevel(level.name);
            current.SelectLevel(level, -1, -1);
            current.Init();
            if (map.Min.x < 0 || map.Min.y < 0)
                issues.Add($"{n} has pieces below (0, 0)");
            else if (map.Min.x > 0 || map.Min.y > 0)
                issues.Add($"{n} doesn't start at (0, 0)");
        });
        map.InitLevel("Uninitialized");
        current.Clear();
        Debug.Log($"Tested {levels.Count} Levels in {gameLevels.Count} Zones for Positioning");
        issues.ForEach(Debug.LogError);
#endif        
        return issues;
    }
}
