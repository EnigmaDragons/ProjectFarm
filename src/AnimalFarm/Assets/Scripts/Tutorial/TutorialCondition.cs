using System;
using UnityEngine;

[Serializable]
public class TutorialCondition
{
    [SerializeField] private TutorialConditonType type;
    [SerializeField] private TilePoint tilePoint;
    [SerializeField] private bool isTrue = true;

    public TutorialConditonType Type => type;
    public TilePoint TilePoint => tilePoint;
    public bool IsTrue => isTrue;
}
