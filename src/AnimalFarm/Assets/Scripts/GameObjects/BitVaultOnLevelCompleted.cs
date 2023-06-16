using System;
using System.Collections;
using UnityEngine;

public class BitVaultOnLevelCompleted : OnMessage<LevelCompleted>
{
    [SerializeField] private CurrentLevelMap map;
    [SerializeField] private Renderer renderer;
    [SerializeField] private Texture _deathMask;
    [SerializeField] private float _secondsForEachSegment = 1;
    [SerializeField] private float _secondsDelayBeforeCompletion = 0;
    [SerializeField] private GameObject collectedStar;
    [SerializeField] private LockBoolVariable gameInputActive;

    private bool _isUnlocking = false;
    private GameObject _heroObject;
    private Vector3 _heroStartPosition;
    private float _t1;
    private float _t2;

    protected override void Execute(LevelCompleted msg)
    {
        gameInputActive.Lock(gameObject);
        _isUnlocking = true;
        _heroObject = map.Hero.gameObject;
        _heroStartPosition = _heroObject.transform.position;
        _t1 = 0;
        _t2 = 0;
    }

    private void Update()
    {
        if (!_isUnlocking)
            return;
        if (_t1 < 1)
        {
            _t1 = Math.Min(1, _t1 + Time.deltaTime / _secondsForEachSegment);
            _heroObject.gameObject.transform.position = Vector3.Lerp(_heroStartPosition, transform.position, _t1);
            if (_t1 == 1)
            {
                _heroObject.gameObject.SetActive(false);
                renderer.material.SetTexture("_DisplacementMask", _deathMask);
                renderer.material.SetFloat("_DefaultShrink", 0);
                renderer.material.SetFloat("_NormalPush", 0);
                renderer.material.SetFloat("_Shrink_Faces_Amplitude", 0);
            }
        }
        else
        {
            _t2 = Math.Min(1, _t2 + Time.deltaTime / _secondsForEachSegment);
            renderer.material.SetFloat("_DefaultShrink", _t2);
            renderer.material.SetFloat("_NormalPush", _t2 * 2);
            if (_t2 == 1)
            {
                _isUnlocking = false;
                gameObject.transform.GetChildren().ForEach(x => x.SetActive(false));
                var star = Instantiate(collectedStar, transform.parent);
                star.transform.localPosition = transform.localPosition;
                Message.Publish(StarCollected.LevelComplete);
                StartCoroutine(DelayedCompletion());
            }
        }
    }

    private IEnumerator DelayedCompletion()
    {
        yield return new WaitForSeconds(_secondsDelayBeforeCompletion);
        Message.Publish(new EndingLevelAnimationFinished());
        gameInputActive.Unlock(gameObject);
    }
}
