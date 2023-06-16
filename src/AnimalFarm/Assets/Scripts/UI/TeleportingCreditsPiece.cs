using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TeleportingCreditsPiece : MonoBehaviour
{
    [SerializeField] private Renderer renderer;
    [SerializeField] private ParticleSystem rendererParticles;
    [SerializeField] private Renderer goToPosition;

    [SerializeField] private float secondsInvisible;
    [SerializeField] private float secondsGrowing;
    [SerializeField] private float secondsVisible;
    [SerializeField] private float secondsShrinking;
    [SerializeField] private FloatReference secondsTeleporting;

    private TeleportingState _state;
    private float _t;

    private Texture _lifeMask;
    private float _lifeShrink;
    private float _lifeNormalPush;
    private float _lifeShrinkFacesAmplitude;
    [SerializeField] private Texture deathMask;
    private const float _deathShrink = 1;
    private const float _deathNormalPush = 1;
    private const float _deathShrinkFacesAmplitude = 0;
    private List<Texture2D> _activeTextures;

    private void Start()
    {
        _activeTextures = new List<Texture2D>();
        randomizeTeleportDirection();
        _lifeMask = renderer.material.GetTexture("_DisplacementMask");
        _lifeShrink = renderer.material.GetFloat("_DefaultShrink");
        _lifeNormalPush = renderer.material.GetFloat("_NormalPush");
        _lifeShrinkFacesAmplitude = renderer.material.GetFloat("_Shrink_Faces_Amplitude");
        _state = Enum.GetValues(typeof(TeleportingState)).Random<TeleportingState>();
        if (_state == TeleportingState.Invisible)
            TransitionToInvisible();
        else if (_state == TeleportingState.Visible)
            TransitionToVisible();
        else if (_state == TeleportingState.Growing)
            TransitionToGrowing();
        else if (_state == TeleportingState.Shrinking)
            TransitionToShrinking();
        if (_state == TeleportingState.Teleport || _state == TeleportingState.TeleportVisible)
            TransitionToTeleportVisible();
        if (_state == TeleportingState.TeleportBack || _state == TeleportingState.TeleportBackVisible)
            TransitionToTeleportBackVisible();
        _t = (float)Rng.Dbl();
    }

    private void randomizeTeleportDirection()
    {
        var randomDirection = Rng.Int(1, 5);
        if (randomDirection == 1)
            goToPosition.transform.localPosition = new Vector3(-2.5f, 0.5f, -0.3f);
        else if (randomDirection == 2)
            goToPosition.transform.localPosition = new Vector3(0.5f, -2.5f, -0.3f);
        else if (randomDirection == 3)
            goToPosition.transform.localPosition = new Vector3(3.5f, 0.5f, -0.3f);
        else if (randomDirection == 4)
            goToPosition.transform.localPosition = new Vector3(0.5f, 3.5f, -0.3f);
    }

    private void Update()
    {
        if (_state == TeleportingState.Invisible)
        {
            _t = Math.Min(1, _t + Time.deltaTime / secondsInvisible);
            if (_t == 1)
                TransitionToGrowing();
        }
        else if (_state == TeleportingState.Visible)
        {
            _t = Math.Min(1, _t + Time.deltaTime / secondsVisible);
            if (_t == 1)
                TransitionToTeleporting();
        }
        else if (_state == TeleportingState.TeleportVisible)
        {
            _t = Math.Min(1, _t + Time.deltaTime / secondsVisible);
            if (_t == 1)
                TransitionToTeleportingBack();
        }
        else if (_state == TeleportingState.TeleportBackVisible)
        {
            _t = Math.Min(1, _t + Time.deltaTime / secondsVisible);
            if (_t == 1)
                TransitionToShrinking();
        }
        else if (_state == TeleportingState.Growing)
        {
            _t = Math.Min(1, _t + Time.deltaTime / secondsGrowing);
            renderer.material.SetFloat("_DefaultShrink", _deathShrink - _t * _deathShrink);
            renderer.material.SetFloat("_NormalPush", _deathNormalPush - _t * _deathNormalPush);
            if (_t == 1)
                TransitionToVisible();
        }
        else if (_state == TeleportingState.Shrinking)
        {
            _t = Math.Min(1, _t + Time.deltaTime / secondsShrinking);
            renderer.material.SetFloat("_DefaultShrink", _t * _deathShrink);
            renderer.material.SetFloat("_NormalPush", _t * _deathNormalPush);
            if (_t == 1)
                TransitionToInvisible();
        }
        else if (_state == TeleportingState.Teleport)
        {
            _t = Math.Min(1, _t + Time.deltaTime / secondsTeleporting.Value);
            UpdateTextures(renderer, Generate(255, Mathf.Max(0, _t - 0.5f) * 2, Mathf.Min(1, _t * 2)), goToPosition, Generate(255, Mathf.Max(0, (1 - _t) - 0.5f) * 2, Mathf.Min(1, (1 - _t) * 2)));
            if (_t == 1)
                TransitionToTeleportVisible();
        }
        else if (_state == TeleportingState.TeleportBack)
        {
            _t = Math.Min(1, _t + Time.deltaTime / secondsTeleporting.Value);
            UpdateTextures(goToPosition, Generate(255, Mathf.Max(0, _t - 0.5f) * 2, Mathf.Min(1, _t * 2)), renderer, Generate(255, Mathf.Max(0, (1 - _t) - 0.5f) * 2, Mathf.Min(1, (1 - _t) * 2)));
            if (_t == 1)
                TransitionToTeleportBackVisible();
        }
    }

    private void UpdateTextures(Renderer from, Texture2D main, Renderer to, Texture2D goTo)
    {
        from.material.SetTexture("_DisplacementMask", main);
        to.material.SetTexture("_DisplacementMask", goTo);
        _activeTextures.ForEach(Destroy);
        _activeTextures = new List<Texture2D> { main, goTo };
    }

    private Texture2D Generate(int size, float whitePercentage, float blackPercentage)
    {
        var texture = new Texture2D(1, size);
        var whitePixels = (int)(size * whitePercentage);
        var blackPixels = (int)(size * (1 - blackPercentage));
        var graidentPixels = size - whitePixels - blackPixels;
        texture.SetPixels(Enumerable.Range(0, size).Select(i =>
        {
            if (i < whitePixels)
                return Color.white;
            if (i >= whitePixels + graidentPixels)
                return Color.black;
            float spotInGradient = i - whitePixels;
            var percentage = 1 - spotInGradient / graidentPixels;
            return new Color(percentage, percentage, percentage, 1);
        }).ToArray());
        texture.Apply();
        return texture;
    }

    private void TransitionToInvisible()
    {
        renderer.material.SetTexture("_DisplacementMask", deathMask);
        renderer.material.SetFloat("_DefaultShrink", _deathShrink);
        renderer.material.SetFloat("_NormalPush", _deathNormalPush);
        renderer.material.SetFloat("_Shrink_Faces_Amplitude", _deathShrinkFacesAmplitude);
        _state = TeleportingState.Invisible;
        _t = 0;
    }

    private void TransitionToGrowing()
    {
        renderer.material.SetTexture("_DisplacementMask", deathMask);
        renderer.material.SetFloat("_DefaultShrink", _deathShrink);
        renderer.material.SetFloat("_NormalPush", _deathNormalPush);
        renderer.material.SetFloat("_Shrink_Faces_Amplitude", _deathShrinkFacesAmplitude);
        _state = TeleportingState.Growing;
        _t = 0;
    }

    private void TransitionToVisible()
    {
        renderer.material.SetTexture("_DisplacementMask", _lifeMask);
        renderer.material.SetFloat("_DefaultShrink", _lifeShrink);
        renderer.material.SetFloat("_NormalPush", _lifeNormalPush);
        renderer.material.SetFloat("_Shrink_Faces_Amplitude", _lifeShrinkFacesAmplitude);
        _state = TeleportingState.Visible;
        _t = 0;
    }

    private void TransitionToShrinking()
    {
        renderer.material.SetTexture("_DisplacementMask", deathMask);
        renderer.material.SetFloat("_DefaultShrink", 0);
        renderer.material.SetFloat("_NormalPush", 0);
        renderer.material.SetFloat("_Shrink_Faces_Amplitude", _deathShrinkFacesAmplitude);
        _state = TeleportingState.Shrinking;
        _t = 0;
    }

    private void TransitionToTeleporting()
    {
        renderer.gameObject.SetActive(true);
        goToPosition.gameObject.SetActive(true);
        rendererParticles.transform.localPosition = renderer.transform.localPosition;
        rendererParticles.transform.rotation = Quaternion.LookRotation(goToPosition.transform.position - renderer.transform.position);
        rendererParticles.Play();
        _state = TeleportingState.Teleport;
        _t = 0;
    }

    private void TransitionToTeleportVisible()
    {
        renderer.gameObject.SetActive(false);
        goToPosition.gameObject.SetActive(true);
        UpdateTextures(goToPosition, Generate(1, 0, 0), renderer, Generate(1, 1, 1));
        _state = TeleportingState.TeleportVisible;
        _t = 0;
    }

    private void TransitionToTeleportingBack()
    {
        renderer.gameObject.SetActive(true);
        goToPosition.gameObject.SetActive(true);
        rendererParticles.transform.localPosition = goToPosition.transform.localPosition;
        rendererParticles.transform.rotation = Quaternion.LookRotation(renderer.transform.position - goToPosition.transform.position);
        rendererParticles.Play();
        _state = TeleportingState.TeleportBack;
        _t = 0;
    }

    private void TransitionToTeleportBackVisible()
    {
        renderer.gameObject.SetActive(true);
        goToPosition.gameObject.SetActive(false);
        UpdateTextures(renderer, Generate(1, 0, 0), goToPosition, Generate(1, 1, 1));
        _state = TeleportingState.TeleportBackVisible;
        _t = 0;
    }
}

public enum TeleportingState
{
    Invisible,
    Growing,
    Visible,
    Teleport,
    TeleportVisible,
    TeleportBack,
    TeleportBackVisible,
    Shrinking
}