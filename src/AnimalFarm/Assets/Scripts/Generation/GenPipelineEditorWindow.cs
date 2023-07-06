#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class GenPipelineEditorWindow : EditorWindow
{
    [MenuItem("AF/Gen")]
    static void EnemiesWanted()
    {
        GetWindow(typeof(GenPipelineEditorWindow)).Show();
    }

    void OnGUI()
    {
        if (GUILayout.Button("Generate"))
        {
            GenPipeline.CreateOne();
            AssetDatabase.Refresh();
        }
    }
}
#endif
