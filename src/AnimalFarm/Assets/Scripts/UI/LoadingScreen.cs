using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen Instance;

    [SerializeField] private GameObject visuals;
    [SerializeField] private RectTransform barFillRectTransform;
    [SerializeField] private TextMeshProUGUI percentLoadedText;
    [SerializeField] private float timeBeforeShowing;
    [SerializeField] private float fadeOutTime;
    [SerializeField] private float minTimeToShow = 1f;
    [SerializeField] private Color startTint;
    [SerializeField] private Color finishTint;
    [SerializeField] private Image image;
    [SerializeField] private Animator animator;
    
    private AsyncOperation _currentLoadingOperation;
    private bool _didTriggerFadeOut;
    private bool _isLoading;
    private Vector3 _barFillLocalScale;
    private float _elapsedTime;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        if (barFillRectTransform != null)
            _barFillLocalScale = barFillRectTransform.localScale;
        Hide();
    }

    private void Update()
    {
        if (!_isLoading) return;
        
        SetProgress(_currentLoadingOperation.progress);
        _elapsedTime += Time.deltaTime;
        if (_elapsedTime >= minTimeToShow)
            _currentLoadingOperation.allowSceneActivation = true;

        WrapUp();
    }

    private void WrapUp()
    {
        if (!_currentLoadingOperation.isDone || _didTriggerFadeOut) return;

        _didTriggerFadeOut = true;
        if (animator != null)
            StartCoroutine(ProcessFadeOut());
        else
            Hide();
    }

    private IEnumerator ProcessFadeOut()
    {
        animator.SetTrigger("FadeOut");
        yield return new WaitForSeconds(fadeOutTime);
        Hide();

    }

    private void SetProgress(float progress)
    {
        var amount = Math.Min(progress, _elapsedTime / minTimeToShow);
        _barFillLocalScale.x = amount;
        if (barFillRectTransform != null)
            barFillRectTransform.localScale = _barFillLocalScale;
        if (percentLoadedText != null)
            percentLoadedText.text = Mathf.CeilToInt(amount * 100) + "%";
        if (image != null)
            image.color = Color.Lerp(startTint, finishTint, amount);
    }

    public void Init(AsyncOperation loadingOperation)
    {
        _elapsedTime = 0;
        StartCoroutine(BeginShow());
        _currentLoadingOperation = loadingOperation;
        if (minTimeToShow > 0)
            _currentLoadingOperation.allowSceneActivation = false;
        SetProgress(0f);
        _didTriggerFadeOut = false;
        _isLoading = true;
    }

    private IEnumerator BeginShow()
    {
        yield return new WaitForSeconds(timeBeforeShowing);
        if (_currentLoadingOperation != null && !_currentLoadingOperation.isDone)
            visuals.SetActive(true);
    }

    public void Hide()
    {
        visuals.SetActive(false);
        _currentLoadingOperation = null;
        _isLoading = false;
    }
}
