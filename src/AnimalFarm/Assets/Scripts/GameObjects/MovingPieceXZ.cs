﻿using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MovingPieceXZ : MonoBehaviour
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

    private MovementType _travelMoveType;
    private Action _onTravelFinished;
    private int _travelDistanceTiles;
    private bool _travelFinishedExecuted;

    private void Awake()
    {
        if (!shouldRotate)
            return;
        
        SetFacingInstant(Facing.Down);
        _facing = (Facing)((Math.Round(rotateTarget.transform.localRotation.eulerAngles.y) / 90) * 90);
        Debug.Log($"Initial Facing: {_facing}");
    }
    
    private void Execute(UndoPieceMoved msg)
    {
        if (msg.Piece == gameObject)
        {
            transform.localPosition = new Vector3(msg.From.X, transform.localPosition.y, msg.From.Y);
            Message.Publish(new PieceDeselected());
            Message.Publish(new PieceSelected(gameObject));
            if (_previousFacings.Count > 0)
                SetFacingInstant(_previousFacings.Pop());
        }
    }

    public void Travel(MovementType travelMoveType, TilePoint from, TilePoint to, Action onFinished)
    {
        Message.Publish(new PieceMovementStarted());
        gameInputActive.Lock(gameObject);
        _travelMoveType = travelMoveType;
        _start = new Vector3(from.X, transform.localPosition.y, from.Y);
        _end = new Vector3(to.X, transform.localPosition.y, to.Y);
        var delta = to - from;
        _travelDistanceTiles =  from.DistanceFrom(to);
        Log.SInfo(LogScopes.GameFlow, $"Traveling {_travelDistanceTiles} Tiles");
        _onTravelFinished = onFinished;
        _travelFinishedExecuted = false;
        _t = 0;
        var newFacing = Facing.Up;
        if (delta.Y > 0)
            newFacing = Facing.Up;
        if (delta.Y < 0)
            newFacing = Facing.Down;
        if (delta.X > 0)
            newFacing = Facing.Right;
        if (delta.X < 0)
            newFacing = Facing.Left;
        _previousFacings.Push(_facing);
        UpdateFacing(newFacing);
    }
    
    private void Execute(PieceMoved msg)
    {
        if (msg.Piece == gameObject)
        {
            _moving = true;
            Message.Publish(new PieceMovementStarted());
            _msg = msg;
            gameInputActive.Lock(gameObject);
            _start = new Vector3(msg.From.X, transform.localPosition.y, msg.From.Y);
            _end = new Vector3(msg.To.X, transform.localPosition.y, msg.To.Y);
            _t = 0;
            var newFacing = Facing.Up;
            if (msg.Delta.Y > 0)
                newFacing = Facing.Up;
            if (msg.Delta.Y < 0)
                newFacing = Facing.Down;
            if (msg.Delta.X > 0)
                newFacing = Facing.Right;
            if (msg.Delta.X < 0)
                newFacing = Facing.Left;
            _previousFacings.Push(_facing);
            UpdateFacing(newFacing);
        }
    }

    private void UpdateFacing(Facing facing)
    {
        Log.SInfo(LogScopes.Movement, $"Set Facing - {facing}");
        if (shouldRotate)
        {
            var newRotationVector = new Vector3(0, (int)facing, 0);
            _facing = facing;

            rotateTarget.transform.DOLocalRotate(newRotationVector, secondsToRotate);
        }
    }

    public void SetFacingInstant(Facing facing)
    {
        Log.SInfo(LogScopes.Movement, $"Set Instant Facing - {facing}");
        if (shouldRotate)
        {
            var newRotationVector = new Vector3(0, (int)facing, 0);
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
                Message.Publish(new PieceMovementFinished(_msg.MovementType));
            }
        }
        if (_onTravelFinished != null)
        {
            _t += Time.deltaTime / (secondsToTravel * _travelDistanceTiles);
            transform.localPosition = Vector3.Lerp(_start, _end, _t);
            if (Vector3.Distance(transform.localPosition, _end) < 0.01)
            {
                transform.localPosition = _end;
                gameInputActive.Unlock(gameObject);
                _moving = false;
                _onTravelFinished();
                _travelFinishedExecuted = true;
            }
        }
    }

    private void LateUpdate()
    {
        if (_onTravelFinished != null && _travelFinishedExecuted)
        {
            _onTravelFinished = null;
            Message.Publish(new PieceMovementFinished(_travelMoveType));
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
