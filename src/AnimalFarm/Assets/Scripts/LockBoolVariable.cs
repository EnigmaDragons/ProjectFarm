using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class LockBoolVariable : ScriptableObject
{
    [SerializeField] private BoolVariable innerVariable;

    private List<GameObject> _locks;

    private void Awake()
    {
        _locks = new List<GameObject>();
        innerVariable.Value = true;
    }

    public void Lock(GameObject obj)
    {
        _locks.Add(obj);
        innerVariable.Value = false;
    }

    public void Unlock(GameObject obj)
    {
        _locks.RemoveAll(x => x == obj);
        _locks.RemoveAll(x => !x);
        innerVariable.Value = !_locks.Any();
    }
}
