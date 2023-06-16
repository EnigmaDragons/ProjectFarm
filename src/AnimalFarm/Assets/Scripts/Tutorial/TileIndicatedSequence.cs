using UnityEngine;

public class TileIndicatedSequence : MonoBehaviour
{
    [SerializeField] private Vector2[] tilePositions;
    [SerializeField] private float secondsPerAction;
    
    private float _secondsLeft;
    private int _index = 0;

    private void Start()
    {
        _secondsLeft = secondsPerAction;
    }

    private void Update()
    {
        _secondsLeft -= Time.deltaTime;
        if (_secondsLeft <= 0 && _index != tilePositions.Length)
        {
            _secondsLeft += secondsPerAction;
            Message.Publish(new TileIndicated(new TilePoint((int)tilePositions[_index].x, (int)tilePositions[_index].y)));
            _index++;
        }
    }
}
