using System.Linq;
using UnityEngine;

public sealed class ToggleBoolVariableOnKeyInputs : MonoBehaviour
{
    [SerializeField] private BoolVariable flag;
    [SerializeField] private KeyCode[] keys;

    private bool _isReady;
    
    void Update ()
    {
        if (Input.anyKey && keys.All(Input.GetKey))
        {
            if (!_isReady) return;
            
            _isReady = false;
            flag.Value = !flag.Value;
        }
        else
            _isReady = true;
    }
}
