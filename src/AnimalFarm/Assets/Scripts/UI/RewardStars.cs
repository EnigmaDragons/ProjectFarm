using UnityEngine;

public class RewardStars : MonoBehaviour
{
    [SerializeField] private CurrentLevelStars stars;
    [SerializeField] private GameObject[] starObjects;

    private void Awake()
    {
        for (var i = 0; i < starObjects.Length; i++)
            starObjects[i].SetActive(i < stars.Count);
    }
}
