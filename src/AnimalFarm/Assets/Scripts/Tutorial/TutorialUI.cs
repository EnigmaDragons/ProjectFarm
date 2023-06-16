using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] private List<Image> fadeables;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float fadeSpeed;

    private List<GameObject> _elements;
    private List<GameObject> _instantiatedElements;
    private float[] _fadableMaxAlphas;
    private string _text;
    private float _opacity;

    public void Start()
    {
        _elements = new List<GameObject>();
        _instantiatedElements = new List<GameObject>();
        _text = "";
        _opacity = 0;
        _fadableMaxAlphas = fadeables.Select(x => x.color.a).ToArray();
        fadeables.ForEach(x => x.color = new Color(x.color.r, x.color.g, x.color.b, _opacity));
        text.color = new Color(text.color.r, text.color.g, text.color.b, _opacity);
    } 

    public void SetLine(TutorialLine line)
    {
        _text = line.Text;
        _elements = line.UIElements;
        _instantiatedElements.ForEach(Destroy);
        _instantiatedElements.Clear();
    }

    public void Clear()
    {
        _text = "";
        _elements.Clear();
        _instantiatedElements.ForEach(Destroy);
        _instantiatedElements.Clear();
    }

    private void Update()
    {
        if (_text != text.text && _opacity > 0)
        {
            _opacity = Math.Max(0, _opacity - Time.deltaTime * fadeSpeed);
            fadeables.ForEach(x => x.color = new Color(x.color.r, x.color.g, x.color.b, _opacity));
            text.color = new Color(text.color.r, text.color.g, text.color.b, _opacity);
        }
        else if (_text != text.text)
        {
            text.text = _text;
        }
        else if (_text != "" && _opacity != 1)
        {
            _opacity = Math.Min(1, _opacity + Time.deltaTime * fadeSpeed);
            
            for (var i = 0; i < fadeables.Count; i++)
            {
                var x = fadeables[i];
                x.color = new Color(x.color.r, x.color.g, x.color.b, _opacity * _fadableMaxAlphas[i]);
            }
            
            text.color = new Color(text.color.r, text.color.g, text.color.b, _opacity);
            if (_opacity > 0.1 && _instantiatedElements.Count == 0)
                _elements.ForEach(x => _instantiatedElements.Add(Instantiate(x, transform)));
        }
    }
}
