using System;
using UnityEngine;

[Serializable]
public struct Boolean3
{
    [SerializeField] private bool x;
    [SerializeField] private bool y;
    [SerializeField] private bool z;
    
    public bool X => x;
    public bool Y => y;
    public bool Z => z;
}
