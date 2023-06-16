using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class ScriptableExtensions
{
    public static List<T> GetAllInstances<T>() where T : ScriptableObject
        => AssetDatabase.FindAssets("t:" + typeof(T).Name)
            .Select(x => AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(x))).ToList();
}
