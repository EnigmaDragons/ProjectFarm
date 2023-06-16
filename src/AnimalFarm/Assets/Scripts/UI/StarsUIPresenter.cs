using System.Linq;
using UnityEngine;

public class StarsUIPresenter : MonoBehaviour
{
    [SerializeField] private CurrentLevelStars currentStars;
    [SerializeField] private StarCounterPresenter[] stars;

    private void OnEnable()
    {
        Message.Subscribe<StarCollected>(_ => AddCollectedStar(), this);
        Message.Subscribe<LevelReset>(_ => Reset(), this);
    }

    private void OnDisable() => Message.Unsubscribe(this);

    private void Reset() => stars.ForEach(s => s.SetState(false));
    private void AddCollectedStar() => Enumerable.Range(0, currentStars.Count).ForEach(i => stars[i].SetState(true));
}
  
