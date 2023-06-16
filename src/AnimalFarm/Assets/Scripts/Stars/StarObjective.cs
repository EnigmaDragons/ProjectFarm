using UnityEngine;

[CreateAssetMenu]
public class StarObjective : ScriptableObject
{
    [SerializeField] private string objective;
    [SerializeField] private int displayOrder = -1;
    
    public string Objective => objective;
    public int DisplayOrder => displayOrder;
}
