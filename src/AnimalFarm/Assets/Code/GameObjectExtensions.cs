using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtensions
{
    public static IEnumerable<GameObject> GetChildren(this Transform t) {
        foreach (Transform childTransform in t) {
            yield return childTransform.gameObject;
        }
    }
    
    public static IEnumerable<GameObject> GetChildren(this GameObject g) {
        foreach (Transform childTransform in g.transform) {
            yield return childTransform.gameObject;
        }
    }
}
