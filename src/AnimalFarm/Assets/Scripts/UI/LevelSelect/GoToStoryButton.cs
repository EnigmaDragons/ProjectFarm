using UnityEngine;

public class GoToStoryButton : MonoBehaviour
{
    [SerializeField] private SaveStorage storage;

    public void Init(GameLevels zone) => gameObject.SetActive(storage.GetLevelsCompletedInZone(zone) > 0);
}
