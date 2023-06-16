using System;
using System.Collections;
using UnityEngine;

public class OnLevelEndFadeOut : OnMessage<LevelCompleted>
{
    [SerializeField] private Texture2D fadeToTexture;
    [SerializeField] private float fadeSeconds;
    [SerializeField] private float delaySeconds;

    private bool _fading;
    private float _alpha = 0f;

    protected override void Execute(LevelCompleted msg)
    {
        StartCoroutine(DelayedFade());
    }

    private IEnumerator DelayedFade()
    {
        yield return new WaitForSeconds(delaySeconds);
        _fading = true;
    }

    private void Update()
    {
        if (_fading)
            _alpha = Math.Min(1, _alpha + Time.deltaTime / fadeSeconds);
    }

    private void OnGUI()
    {
        if (_fading)
        {
            GUI.color = new Color(0, 0, 0, _alpha);
            GUI.depth = -1000;
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeToTexture);
        }
    }
}
