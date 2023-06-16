using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerSurvey : ScriptableObject
{
    public bool HasSurvey;
    public string Question;
    public List<string> Answers;
}
