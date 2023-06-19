using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class HintButton : MonoBehaviour
{
    [SerializeField] private CurrentLevelMap map;
    [SerializeField] private TextMeshProUGUI cantWin;
    [SerializeField] private LockBoolVariable gameInputActiveLock;
    [SerializeField] private BoolVariable gameInputActive;
    [SerializeField] private float _secondsMessageTransition;
    [SerializeField] private float _secondsDelayMove;
    [SerializeField] private CurrentSelectedPiece _piece;

    private AIBrute _ai;
    private bool _showingError;
    private bool _makingMove;
    private float _t;

    private void Start()
    {
        _ai = new AIBrute();
    }

    private void Update()
    {
        if (_showingError)
        {
            var a = Math.Min(1, cantWin.color.a + Time.deltaTime / _secondsMessageTransition);
            cantWin.color = new Color(cantWin.color.r, cantWin.color.g, cantWin.color.b, a);
            if (a == 1)
                _showingError = false;
        }
        else
            cantWin.color = new Color(cantWin.color.r, cantWin.color.g, cantWin.color.b, Math.Max(0, cantWin.color.a - Time.deltaTime / _secondsMessageTransition));

        if (_makingMove)
        {
            _t = Math.Max(0, _t - Time.deltaTime);
            if (_t == 0)
            {
                _makingMove = false;
                gameInputActiveLock.Unlock(gameObject);
                Message.Publish(new PieceMoved(MovementType.Jump, _piece.Selected.Value, new TilePoint(_ai.NextMove.FromX, _ai.NextMove.FromY), new TilePoint(_ai.NextMove.ToX, _ai.NextMove.ToY), -1));
            }
        }
    }

    public void Hint()
    {
        if (!gameInputActive.Value)
            return;
        gameInputActiveLock.Lock(gameObject);
        StartCoroutine(Calculate());
    }

    private IEnumerator Calculate()
    {
        yield return new WaitUntil(() => _ai.CalculateSolution(map.GetSnapshot()));
        if (_ai.CanWin)
        {
            
            Message.Publish(new TileIndicated(new TilePoint(_ai.NextMove.FromX, _ai.NextMove.FromY)));
            _makingMove = true;
            _t = _secondsDelayMove;
        }
        else
        {
            gameInputActiveLock.Unlock(gameObject);
            _showingError = true;
        }
    }
}
