using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TutorialLine
{
    [SerializeField] private string text;
    [SerializeField] private List<TutorialCondition> conditions;
    [SerializeField] private List<GameObject> uiElements;
    [SerializeField] private List<PositionedElement> elements;

    public string Text => text;
    public List<TutorialCondition> Conditions => conditions;
    public List<GameObject> UIElements => uiElements;
    public List<PositionedElement> Elements => elements;
}
