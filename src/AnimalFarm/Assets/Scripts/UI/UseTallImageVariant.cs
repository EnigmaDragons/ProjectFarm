using UnityEngine;
using UnityEngine.UI;

public class UseTallImageVariant : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Sprite variant;

    void Awake() => (Screen.height > Screen.width).If(() => image.sprite = variant);
}
  
