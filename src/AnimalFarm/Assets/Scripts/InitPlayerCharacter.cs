using UnityEngine;

public sealed class InitPlayerCharacter : MonoBehaviour
{
    [SerializeField] private BoolVariable useFemale;
    [SerializeField] private SaveStorage storage;

    private void Awake()
    {
        useFemale.Value = storage.GetUseFemale();
    }
}
