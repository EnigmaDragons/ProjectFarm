using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CreditStrings : MonoBehaviour
{
    [SerializeField] private List<string> Strings;
    [SerializeField] private TextMeshProUGUI Text;
    [SerializeField] private GameObject[] ExtraneousObjects;
    [SerializeField] private UnityEvent OnCreditsFinished;

    private int _index;
    private bool _finished;

    private void Start()
    {
        _index = 0;
        _finished = false;
        Text.text = Strings[_index];
    }

    public void Next()
    {
        if (_finished)
            return;
        _index++;
        Text.text = _index >= Strings.Count ? "" : Strings[_index];
        if (_index == Strings.Count)
        {
            ExtraneousObjects.ForEach(x => x.SetActive(false));
            _finished = true;
            OnCreditsFinished.Invoke();
        }
    }
}
