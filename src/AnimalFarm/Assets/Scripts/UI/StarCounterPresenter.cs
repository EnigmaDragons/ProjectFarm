using UnityEngine;
using UnityEngine.UI;

public class StarCounterPresenter : MonoBehaviour
{
    [SerializeField] private Sprite withStar;
    [SerializeField] private Sprite noStar;
    [SerializeField] private Image image;
    
    public void SetState(bool hasStar)
    {
        image.sprite = hasStar ? withStar : noStar;
    }
}
