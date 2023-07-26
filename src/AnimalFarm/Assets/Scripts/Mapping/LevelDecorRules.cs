using System;
using UnityEngine;

[Serializable]
public class LevelDecorRules
{
    public GameObject Prototype;
    public bool IsUnique;
    public bool IsTall;
    public float Odds = 0.05f;
}
