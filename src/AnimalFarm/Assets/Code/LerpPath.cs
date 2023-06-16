using UnityEngine;

public class LerpPath : MonoBehaviour
{
    [SerializeField] private GameObject obj;
    [SerializeField] private LerpPathNode[] nodes;

    private bool _isFinished;

    private Vector3 _lastPos;
    private Quaternion _lastRotation;
    private int _targetIndex = -1;
    private float _remainingTime = 0;


    private void Update()
    {
        if (_isFinished)
            return;

        if (_targetIndex >= nodes.Length)
        {
            _isFinished = true;
            return;
        }

        _remainingTime -= Time.deltaTime;
        
        if (_remainingTime <= 0 && _targetIndex < nodes.Length)
        {
            _targetIndex++;
            _remainingTime = nodes[_targetIndex].duration;
            _lastPos = obj.transform.position;
            _lastRotation = obj.transform.rotation;
        }
        
        //obj.transform.position = 

    }
}
