using System;
using UnityEngine;

[CreateAssetMenu]
public class GameLevel : ScriptableObject
{
    [ReadOnly, SerializeField] private string guid = Guid.NewGuid().ToString();
    [SerializeField] private string levelName;
    [SerializeField] private GameObject prefab;
    [SerializeField] private bool isTutorial = false;

    public string Name => levelName;
    public GameObject Prefab => prefab;
    public bool IsTutorial => isTutorial;
    public string Id => guid;
}
