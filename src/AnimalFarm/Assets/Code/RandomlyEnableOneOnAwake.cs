using UnityEngine;

public class RandomlyEnableOneOnAwake : MonoBehaviour
{
    [SerializeField] private GameObject[] options;

    private void Awake()
    {
        options.ForEach(o => o.SetActive(false));
        options.Random().SetActive(true);
    }
}
