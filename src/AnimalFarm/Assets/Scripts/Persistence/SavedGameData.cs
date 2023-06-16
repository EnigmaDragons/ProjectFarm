using System;
using System.Collections.Generic;

[Serializable]
public sealed class SavedGameData
{
    public static string CurrentDataVersion => "0.7.8";

    public string SaveDataVersion = CurrentDataVersion;
    public int ActiveZone = 0;
    public string ActiveCampaignName = "";
    public string SelectedCharacter = "None";
    public CampaignsProgressData Campaigns = new CampaignsProgressData();
    public SettingsData Settings = new SettingsData();
    public bool HasWon = false;
    public AchievementData Achievements = new AchievementData();
    public List<int> ZonesVisited = new List<int>();

    public CampaignLevelScores ActiveCampaign => Campaigns[ActiveCampaignName];
}

[Serializable]
public sealed class CampaignsProgressData : SerializableDictionary<string, CampaignLevelScores> {}

[Serializable]
public sealed class CampaignLevelScores : SerializableDictionary<string, int> {}

[Serializable]
public sealed class SettingsData
{
    public bool ShowMovementHints = true;
    public bool AutoSkipStory = false;
    public bool UseFemale = true;
}

[Serializable]
public sealed class AchievementData
{
    public List<List<TilePoint>> RoutesTakenOnLevel1 = new List<List<TilePoint>>();
}
