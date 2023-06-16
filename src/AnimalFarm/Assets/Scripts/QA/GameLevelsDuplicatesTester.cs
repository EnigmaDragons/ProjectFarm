using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameLevelsDuplicatesTester : RuntimeAcceptanceTest
{
    protected override List<string> GetAllIssues()
    {
        var issues = new List<string>();
        var levels = new HashSet<string>();
#if UNITY_EDITOR        
        var gameLevels = UnityResourceUtils.FindAssetsByType<GameLevels>();
        gameLevels.SelectMany(zone => zone.Value).ForEach(level =>
        {
            var key = level.GetInstanceID().ToString();
            if (!levels.Add(key))
                issues.Add($"Duplicate of {level.Name} - {level.GetInstanceID()}");
        });
        Debug.Log($"Tested {levels.Count} Levels in {gameLevels.Count} Zones for Duplicates");
#endif
        return issues;
    }
}
