using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

public class StoryExporter
{
    private const string StartLevel = "STARTING LEVEL";
    private const string DoingLevel = "LEVEL COMPLETED";

    [MenuItem("EnigmaDragons/Export Story")]
    public static void ExportStory()
    {
        ScriptableExtensions.GetAllInstances<Campaign>().ForEach(ExportStory);
    }

    private static void ExportStory(Campaign zones)
    {
        var path = EditorUtility.SaveFilePanel("Save Story To", "", zones.Name + ".txt", "txt");
        if (path.Length == 0)
            return;
        File.WriteAllLines(path, zones.Value
            .SelectMany((zone, zoneI) => zone.Story
                .SelectMany((story, storyI) => new List<string> { $"SELECTED STORY {zoneI + 1}-{storyI + 1}: {story.Intro.DialogueName}", "" }
                    .Concat(DialogueToStrings(story.Intro.Lines.Where(x => x.Type == DialogueLineType.StatementOnly).ToArray()))
                    .Concat(new List<string> { "", $"COMPLETED LEVEL {zoneI + 1}-{storyI + 1}: {story.Intro.DialogueName}", "" })
                    .Concat(DialogueToStrings(story.Outro.Lines.Where(x => x.Type == DialogueLineType.StatementOnly).ToArray()))
                    .Concat(new List<string> { "" }))));
    }

    private static List<string> DialogueToStrings(DialogueLine[] dialogue)
    {
        return dialogue.Select(x => $"{x.Character.Name}: {x.Text}").ToList();
    }
}