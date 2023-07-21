using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class PulseImageAlpha : MonoBehaviour
{
    [SerializeField] private float minAlpha = 0.5f;
    [SerializeField] private float maxAlpha = 0.8f;

    private Image _image;
    
    private void Awake()
    {
        _image = GetComponent<Image>();
    }
    
    private void Update()
    {
        if (_image == null)
            return;
        
        _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, Mathf.Lerp(minAlpha, maxAlpha, Mathf.PingPong(Time.time, 1)));
    }
}
