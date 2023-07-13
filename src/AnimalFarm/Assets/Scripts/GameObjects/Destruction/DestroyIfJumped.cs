using System;
using UnityEngine;

public class DestroyIfJumped : OnMessage<PieceMoved>
{
    [SerializeField] private Renderer[] renderers;
    [SerializeField] private Texture deathMask;
    [SerializeField] private float secondsTilDeath = 1;
    
    private bool _isDying = false;
    private float _t;
    private Texture _lifeMask;
    private float _defaultShrink;
    private float _normalPush;
    private float _shrinkFacesAmplitude;

    public void Revert()
    {
        _isDying = false;
        renderers.ForEach(renderer =>
        {
            renderer.material.SetTexture("_DisplacementMask", _lifeMask);
            renderer.material.SetFloat("_DefaultShrink", _defaultShrink);
            renderer.material.SetFloat("_NormalPush", _normalPush);
            renderer.material.SetFloat("_Shrink_Faces_Amplitude", _shrinkFacesAmplitude);
        });
        gameObject.SetActive(true);
    }

    protected override void Execute(PieceMoved msg)
    {
        if (!msg.HasJumpedOver(gameObject)) return;
        
        Message.Publish(new ObjectDestroyed(gameObject, msg.MoveNumber, true));
        StartDying();
    }

    private void StartDying()
    {
        _isDying = true;
        _t = 0;
        renderers.ForEach(SetupForDeath);
    }

    private void SetupForDeath(Renderer renderer)
    {
        _lifeMask = renderer.material.GetTexture("_DisplacementMask");
        _defaultShrink = renderer.material.GetFloat("_DefaultShrink");
        _normalPush = renderer.material.GetFloat("_NormalPush");
        _shrinkFacesAmplitude = renderer.material.GetFloat("_Shrink_Faces_Amplitude");
        renderer.material.SetTexture("_DisplacementMask", deathMask);
        renderer.material.SetFloat("_DefaultShrink", 0);
        renderer.material.SetFloat("_NormalPush", 0);
        renderer.material.SetFloat("_Shrink_Faces_Amplitude", 0);
    }

    private void Update()
    {
        if (!_isDying)
            return;
        _t = Math.Min(1, _t + Time.deltaTime / secondsTilDeath);
        renderers.ForEach(ApproachDeath);
        if (_t == 1)
            gameObject.SetActive(false);
    }

    private void ApproachDeath(Renderer renderer)
    {
        renderer.material.SetFloat("_DefaultShrink", _t);
        renderer.material.SetFloat("_NormalPush", _t);
    }
}
