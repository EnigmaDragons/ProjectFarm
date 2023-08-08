using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private FloatReference rotateDelayBeforeMove = new FloatReference(0.15f);
    [SerializeField] private bool shouldAnimate = false;
    [SerializeField] private int walkAnimation = 1;
    [SerializeField] private int runAnimation = 2;
    
    private Facing _facing;
    private bool _moving = false;
    private PieceMoved _msg;
    private Vector3 _start;
    private Vector3 _end;
    private float _t;
    private float _speedFactor = 1;
    private readonly Stack<Facing> _previousFacings = new Stack<Facing>(200);

    private int _travelMoveNumber;
    private MovementType _travelMoveType;
    private Action _onTravelFinished;
    private int _travelDistanceTiles;
    private bool _travelFinishedExecuted;

    private Animator _animator;
    
    private void Awake()
    {
        if (shouldRotate)
        {
            SetFacingInstant(Facing.Down);
            _facing = (Facing)((Math.Round(rotateTarget.transform.localRotation.eulerAngles.y) / 90) * 90);
            Debug.Log($"Initial Facing: {_facing}");
        }
    }

    private void UpdateAnimator()
    {
        if (shouldAnimate) 
            _animator = GetComponentsInChildren<Animator>().SingleOrDefault(x => x.gameObject.activeInHierarchy);
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

    public void Travel(MovementType travelMoveType, int travelMoveNumber, TilePoint from, TilePoint to, Action onFinished)
    {
        Message.Publish(new PieceMovementStarted());
        gameInputActive.Lock(gameObject);
        _travelMoveType = travelMoveType;
        _travelMoveNumber = travelMoveNumber;
        _start = new Vector3(from.X, transform.localPosition.y, from.Y);
        _end = new Vector3(to.X, transform.localPosition.y, to.Y);
        var delta = to - from;
        _travelDistanceTiles =  from.DistanceFrom(to);
        Log.SInfo(LogScopes.GameFlow, $"Traveling {_travelDistanceTiles} Tiles");
        _onTravelFinished = onFinished;
        _travelFinishedExecuted = false;
        _t = 0;
        FaceTowards(delta);
    }

    public void Move(MovementType travelMoveType, int travelMoveNumber, TilePoint from, TilePoint to, float speedFactor = 1f)
    {
        var delay = FaceTowards(to - from) ? rotateDelayBeforeMove : 0f;
        StartCoroutine(BeginMovingAfterDelay(new PieceMoved(travelMoveType, gameObject, from, to, travelMoveNumber), delay));
    }

    public void SetAnimation(int val)
    {
        _animator.SetInteger("animation", val);
    }
    
    private void Execute(PieceMoved msg)
    {
        if (msg.Piece != gameObject)
            return;

        if (msg.MovementType == MovementType.Activate)
        {
            FaceTowards(msg.To - msg.From);
            return;
        }

        if (msg.MovementType == MovementType.Genius)
            return;

        var delay = FaceTowards(msg.To - msg.From) ? rotateDelayBeforeMove : 0f;
        StartCoroutine(BeginMovingAfterDelay(msg, delay));
    }

    private IEnumerator BeginMovingAfterDelay(PieceMoved msg, float delay, float speedFactor = 1f)
    {
        _speedFactor = speedFactor;
        yield return new WaitForSeconds(delay);
        UpdateAnimator();
        BeginMoving(msg);
    }

    private void BeginMoving(PieceMoved msg)
    {
        _moving = true;
        Message.Publish(new PieceMovementStarted());
        _msg = msg;
        gameInputActive.Lock(gameObject);
        var distance = msg.Delta.TotalMagnitude();
        if (_animator != null)
            if (distance == 1)
                _animator.SetInteger("animation", walkAnimation);
            else 
                _animator.SetInteger("animation", runAnimation);
        _start = new Vector3(msg.From.X, transform.localPosition.y, msg.From.Y);
        _end = new Vector3(msg.To.X, transform.localPosition.y, msg.To.Y);
        _t = 0;
    }

    public bool FaceTowards(TilePoint delta)
    {
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
        var facingChanged = _facing != newFacing && shouldRotate;
        if (facingChanged)
            UpdateFacing(newFacing);
        return facingChanged;
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
            _t += Time.deltaTime / (secondsToTravel / _speedFactor);
            transform.localPosition = Vector3.Lerp(_start, _end, _t);
            if (Vector3.Distance(transform.localPosition, _end) < 0.01)
            {
                if (_animator != null)
                    _animator.SetInteger("animation", 0);
                transform.localPosition = _end;
                map.Move(_msg.Piece, _msg.From, _msg.To);
                gameInputActive.Unlock(gameObject);
                _moving = false;
                Message.Publish(new PieceMovementFinished(_msg.MovementType, gameObject, _msg.MoveNumber));
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
                if (_animator != null)
                    _animator.SetInteger("animation", 0);
            }
        }
    }

    private void LateUpdate()
    {
        if (_onTravelFinished != null && _travelFinishedExecuted)
        {
            _onTravelFinished = null;
            Message.Publish(new PieceMovementFinished(_travelMoveType, gameObject, _travelMoveNumber));
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
