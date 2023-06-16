using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MovingPiece : MonoBehaviour
{
    [SerializeField] private LockBoolVariable gameInputActive;
    [SerializeField] private FloatReference secondsToTravel;
    [SerializeField] private CurrentLevelMap map;
    [SerializeField] private FloatReference secondsToRotate = new FloatReference(0.16f);
    [SerializeField] private GameObject rotateTarget;
    [SerializeField] private bool shouldRotate;

    private Facing _facing;
    private bool _moving = false;
    private PieceMoved _msg;
    private Vector3 _start;
    private Vector3 _end;
    private float _t;
    private readonly Stack<Facing> _previousFacings = new Stack<Facing>(200);

    private void Awake()
    {
        if (!shouldRotate)
            return;
        
        _facing = (Facing)((Math.Round(rotateTarget.transform.localRotation.eulerAngles.z) / 90) * 90);
        Debug.Log($"Facing: {_facing}");
    }
    
    private void Execute(UndoPieceMoved msg)
    {
        if (msg.Piece == gameObject)
        {
            transform.localPosition = new Vector3(msg.From.X, msg.From.Y, transform.localPosition.z);
            Message.Publish(new PieceDeselected());
            Message.Publish(new PieceSelected(gameObject));
            if (_previousFacings.Count > 0)
                SetRotationInstant(_previousFacings.Pop());
        }
    }

    private void Execute(PieceMoved msg)
    {
        if (msg.Piece == gameObject)
        {
            _moving = true;
            Message.Publish(new PieceMovementStarted());
            _msg = msg;
            gameInputActive.Lock(gameObject);
            _start = new Vector3(msg.From.X, msg.From.Y, transform.localPosition.z);
            _end = new Vector3(msg.To.X, msg.To.Y, transform.localPosition.z);
            _t = 0;
            var newFacing = Facing.Up;
            if (msg.Delta.Y > 0)
                newFacing = Facing.Right;
            if (msg.Delta.Y < 0)
                newFacing = Facing.Left;
            if (msg.Delta.X > 0)
                newFacing = Facing.Up;
            if (msg.Delta.X < 0)
                newFacing = Facing.Down;
            _previousFacings.Push(_facing);
            UpdateRotation(newFacing);
        }
    }

    private void UpdateRotation(Facing facing)
    {
        if (shouldRotate)
        {
            var newRotationVector = new Vector3(0, 0, (int)facing);
            _facing = facing;

            rotateTarget.transform.DOLocalRotate(newRotationVector, secondsToRotate);
        }
    }

    private void SetRotationInstant(Facing facing)
    {
        if (shouldRotate)
        {
            var newRotationVector = new Vector3(0, 0, (int)facing);
            _facing = facing;

            rotateTarget.transform.localRotation = Quaternion.Euler(newRotationVector);
        }
    }

    private void Update()
    {
        if (_moving)
        {
            _t += Time.deltaTime / secondsToTravel;
            transform.localPosition = Vector3.Lerp(_start, _end, _t);
            if (Vector3.Distance(transform.localPosition, _end) < 0.01)
            {
                transform.localPosition = _end;
                map.Move(_msg.Piece, _msg.From, _msg.To);
                gameInputActive.Unlock(gameObject);
                _moving = false;
                Message.Publish(new PieceMovementFinished());
            }
        }
    }

    private void OnEnable()
    {
        _moving = false;
        Message.Subscribe<PieceMoved>(Execute, this);
        Message.Subscribe<UndoPieceMoved>(Execute, this);
    }

    private void OnDisable()
    {
        gameInputActive.Unlock(gameObject);
        Message.Unsubscribe(this);
    }
}
