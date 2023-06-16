using UnityEngine;

public class FindSceneCamera : MonoBehaviour
{
    [SerializeField] private Canvas canvasThatNeedsACamera;

    private void Start() => canvasThatNeedsACamera.worldCamera = Camera.main;
}
