
using UnityEngine;

public sealed class HideOnThreeStars : MonoBehaviour
{
    [SerializeField] private StarCounter stars;

    private void Awake() => gameObject.SetActive(stars.NumStars != 3);
}
