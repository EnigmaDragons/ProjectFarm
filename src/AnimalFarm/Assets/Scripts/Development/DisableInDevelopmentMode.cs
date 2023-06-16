using UnityEngine;

public class DisableInDevelopmentMode : MonoBehaviour
{
    [SerializeField] private GameObject obj;
    [SerializeField] private BoolReference developmentModeActive;

    private void Awake()
    {
        if (developmentModeActive)
            obj.SetActive(false);
    }
}
