using UnityEngine;
using UnityEngine.UI;

public class Counter : MonoBehaviour
{
    private Text counterText;
    public Slider Slider;
    
    void Start()
    {
        counterText = GetComponent<Text>();
    }

    void Update()
    {
        counterText.text = Slider.value.ToString("0");
    }
}
