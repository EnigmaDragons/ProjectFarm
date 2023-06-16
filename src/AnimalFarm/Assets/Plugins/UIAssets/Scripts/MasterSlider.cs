using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MasterSlider : MonoBehaviour
{
    public List<Slider> Sliders;
    // Start is called before the first frame update
    void Start()
    {
        foreach(Transform c in transform.parent)
        {
            Sliders.Add(c.GetComponent<Slider>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach(Slider s in Sliders)
        {
            s.value = GetComponent<Slider>().value;
        }
    }
}
