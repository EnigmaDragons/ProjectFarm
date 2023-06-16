using System.Collections.Generic;
using UnityEngine;

public class MovementEnabled : MonoBehaviour
{
    [SerializeField] private List<MovementType> types;

    public List<MovementType> Types => types;
}
