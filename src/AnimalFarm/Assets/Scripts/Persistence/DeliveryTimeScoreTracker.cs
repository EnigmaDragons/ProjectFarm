using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public sealed class DeliveryTimeScoreTracker : ScriptableObject
{
    [SerializeField] private List<int> numStars = new List<int>();

    public int TotalStars => numStars.Sum();
    public int PossibleStars => numStars.Count * 3;
    
    public void Init() => numStars = new List<int>();

    public void RecordLevel(int happiness) => numStars.Add(happiness);
}
