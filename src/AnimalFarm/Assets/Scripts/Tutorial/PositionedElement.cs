using System;
using UnityEngine;

[Serializable]
public class PositionedElement
{
    [SerializeField] private GameObject element;
    [SerializeField] private Vector3 position;

    public GameObject Element => element;
    public Vector3 Position => position;
}
