using UnityEngine;
using UnityEngine.UI;

public class LogoController : MonoBehaviour
{
    [SerializeField] private Navigator navigator;
    [SerializeField] private Image image;
    [SerializeField] private float showDuration = 2f;
    [SerializeField] private float transitionDuration = 0.75f;

    private Color targetColor;
    private Color targetTransparent;

    private float _fadingInFinishedInSeconds;
    private float _startFadingOutInSeconds;
    private float _finishInSeconds;
    private bool _finishedCurrent;
    private bool _startedLoading;

    private void Awake()
    {
        targetColor = image.color;
        targetTransparent = new Color(targetColor.r, targetColor.g, targetColor.b, 0f);
        image.color = targetTransparent;
        BeginAnim();
    }

    private bool AnyRelevantButtonPress() => Input.GetButton("Fire1") || Input.GetButton("Cancel") ||
                                             Input.GetButton("Submit") || Input.GetButton("Jump") ||
                                             Input.GetButton("Fire2");
    private bool AnyMouseButtonDown() => Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1);
    
    private void FixedUpdate()
    {
        UpdateCounters();
        UpdatePresentation();
        if (!_startedLoading && _finishedCurrent || AnyMouseButtonDown() || AnyRelevantButtonPress())
        {
            _startedLoading = true;
            NavigateToMainMenu();
        }
    }

    private void BeginAnim()
    {
        _finishedCurrent = false;
        _finishInSeconds = showDuration + transitionDuration * 2;
        _fadingInFinishedInSeconds = transitionDuration;
        _startFadingOutInSeconds = transitionDuration + showDuration;
    }

    private void UpdatePresentation()
    {
        if (_finishedCurrent)
            return;

        if (_fadingInFinishedInSeconds > 0.01f)
            image.color = Color.Lerp(targetTransparent, targetColor, (transitionDuration - _fadingInFinishedInSeconds) / transitionDuration);
        if (_startFadingOutInSeconds < 0.1f)
            image.color = Color.Lerp(targetColor, targetTransparent, (transitionDuration - _finishInSeconds) / transitionDuration);
        if (_finishInSeconds < 0.01f)
            _finishedCurrent = true;
    }

    private void UpdateCounters()
    {
        _fadingInFinishedInSeconds = Mathf.Max(0, _fadingInFinishedInSeconds - Time.deltaTime);
        _startFadingOutInSeconds = Mathf.Max(0, _startFadingOutInSeconds - Time.deltaTime);
        _finishInSeconds = Mathf.Max(0, _finishInSeconds - Time.deltaTime);
    }

    private void NavigateToMainMenu() => navigator.NavigateToMainMenu();
}
