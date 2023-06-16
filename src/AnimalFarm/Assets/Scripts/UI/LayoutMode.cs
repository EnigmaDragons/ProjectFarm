using System;
using UnityEngine;

[CreateAssetMenu]
public class LayoutMode : ScriptableObject
{
    public bool IsTall => Screen.height > Screen.width;
    public bool IsWide => Screen.height < Screen.width;
    public ResolutionAspectRatio AspectRatio => Is4By3Ratio ? ResolutionAspectRatio.FourByThree : Is39By18Ratio ? ResolutionAspectRatio.ThirtyNineByEighteen : ResolutionAspectRatio.Default;
    private bool Is4By3Ratio => Math.Abs((float)Screen.width / (float)Screen.height - 4f / 3f) < 0.1 || Math.Abs((float)Screen.height / (float)Screen.width - 4f / 3f) < 0.1;
    private bool Is39By18Ratio => Math.Abs((float)Screen.width / (float)Screen.height - 39f / 18f) < 0.1 || Math.Abs((float)Screen.height / (float)Screen.width - 39f / 18f) < 0.1;
}
