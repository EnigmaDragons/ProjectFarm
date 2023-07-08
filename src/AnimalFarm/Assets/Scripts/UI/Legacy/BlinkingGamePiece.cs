using System;
using UnityEngine;
using Random = System.Random;

public class BlinkingGamePiece : MonoBehaviour
{
    [SerializeField] private Renderer renderer;
    [SerializeField] private float secondsInvisible;
    [SerializeField] private float secondsGrowing;
    [SerializeField] private float secondsVisible;
    [SerializeField] private float secondsShrinking;

    private BlinkingState _state;
    private float _t;

    private Texture _lifeMask;
    private float _lifeShrink;
    private float _lifeNormalPush;
    private float _lifeShrinkFacesAmplitude;
    [SerializeField] private Texture deathMask;
    private const float _deathShrink = 1;
    private const float _deathNormalPush = 1;
    private const float _deathShrinkFacesAmplitude = 0;

    private void Start()
    {
        _lifeMask = renderer.material.GetTexture("_DisplacementMask");
        _lifeShrink = renderer.material.GetFloat("_DefaultShrink");
        _lifeNormalPush = renderer.material.GetFloat("_NormalPush");
        _lifeShrinkFacesAmplitude = renderer.material.GetFloat("_Shrink_Faces_Amplitude");
        _state = Enum.GetValues(typeof(BlinkingState)).Random<BlinkingState>();
        if (_state == BlinkingState.Invisible)
            TransitionToInvisible();
        else if (_state == BlinkingState.Visible)
            TransitionToVisible();
        else if (_state == BlinkingState.Growing)
            TransitionToGrowing();
        else if (_state == BlinkingState.Shrinking)
            TransitionToShrinking();
        _t = (float)Rng.Dbl();
    }

    private void Update()
    {
        if (_state == BlinkingState.Invisible)
        {
            _t = Math.Min(1, _t + Time.deltaTime / secondsInvisible);
            if (_t == 1)
                TransitionToGrowing();
        }
        else if (_state == BlinkingState.Visible)
        {
            _t = Math.Min(1, _t + Time.deltaTime / secondsVisible);
            if (_t == 1)
                TransitionToShrinking();
        }
        else if (_state == BlinkingState.Growing)
        {
            _t = Math.Min(1, _t + Time.deltaTime / secondsGrowing);
            renderer.material.SetFloat("_DefaultShrink", _deathShrink - _t * _deathShrink);
            renderer.material.SetFloat("_NormalPush", _deathNormalPush - _t * _deathNormalPush);
            if (_t == 1)
                TransitionToVisible();
        }
        else if (_state == BlinkingState.Shrinking)
        {
            _t = Math.Min(1, _t + Time.deltaTime / secondsShrinking);
            renderer.material.SetFloat("_DefaultShrink", _t * _deathShrink);
            renderer.material.SetFloat("_NormalPush", _t * _deathNormalPush);
            if (_t == 1)
                TransitionToInvisible();
        }
    }

    private void TransitionToInvisible()
    {
        renderer.material.SetTexture("_DisplacementMask", deathMask);
        renderer.material.SetFloat("_DefaultShrink", _deathShrink);
        renderer.material.SetFloat("_NormalPush", _deathNormalPush);
        renderer.material.SetFloat("_Shrink_Faces_Amplitude", _deathShrinkFacesAmplitude);
        _state = BlinkingState.Invisible;
        _t = 0;
    }

    private void TransitionToGrowing()
    {
        renderer.material.SetTexture("_DisplacementMask", deathMask);
        renderer.material.SetFloat("_DefaultShrink", _deathShrink);
        renderer.material.SetFloat("_NormalPush", _deathNormalPush);
        renderer.material.SetFloat("_Shrink_Faces_Amplitude", _deathShrinkFacesAmplitude);
        _state = BlinkingState.Growing;
        _t = 0;
    }

    private void TransitionToVisible()
    {
        renderer.material.SetTexture("_DisplacementMask", _lifeMask);
        renderer.material.SetFloat("_DefaultShrink", _lifeShrink);
        renderer.material.SetFloat("_NormalPush", _lifeNormalPush);
        renderer.material.SetFloat("_Shrink_Faces_Amplitude", _lifeShrinkFacesAmplitude);
        _state = BlinkingState.Visible;
        _t = 0;
    }

    private void TransitionToShrinking()
    {
        renderer.material.SetTexture("_DisplacementMask", deathMask);
        renderer.material.SetFloat("_DefaultShrink", 0);
        renderer.material.SetFloat("_NormalPush", 0);
        renderer.material.SetFloat("_Shrink_Faces_Amplitude", _deathShrinkFacesAmplitude);
        _state = BlinkingState.Shrinking;
        _t = 0;
    }
}

public enum BlinkingState
{
    Invisible,
    Growing,
    Visible,
    Shrinking
}
