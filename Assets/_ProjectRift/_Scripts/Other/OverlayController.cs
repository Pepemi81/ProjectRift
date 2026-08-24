using System;
using DG.Tweening;
using UnityEngine;

public enum AlphaOnEnableBehaviour
{
    DoNothing,
    ShowFromCurrent,
    HideFromCurrent,
    Show,
    Hide
}

public abstract class OverlayController : MonoBehaviour
{
    [Header("Alpha")]
    [SerializeField, Range(0f, 1f)] private float _hiddenAlpha;
    [SerializeField, Range(0f, 1f)] private float _visibleAlpha = 1f;

    [Header("Animation")]
    [SerializeField] private AlphaOnEnableBehaviour _onEnableBehaviour = AlphaOnEnableBehaviour.Show;
    [SerializeField, Min(0f)] private float _duration = 3f;
    [SerializeField] private Ease _ease = Ease.InOutQuad;
    [SerializeField] private bool _useUnscaledTime;

    private Tween _alphaTween;
    private float _currentAlpha;

    public float CurrentAlpha => _currentAlpha;
    public bool IsAnimating => _alphaTween != null && _alphaTween.IsActive() && _alphaTween.IsPlaying();

    protected virtual void Awake()
    {
        Initialize();
    }

    protected virtual void OnEnable()
    {
        Initialize();
        ApplyOnEnableBehaviour();
    }

    protected virtual void OnDisable()
    {
        KillTween();
    }

    // Sobrecargas para inspector
    public void Show() => Show(null);
    public void ShowFromCurrent() => ShowFromCurrent(null);
    public void Hide() => Hide(null);
    public void HideFromCurrent() => HideFromCurrent(null);
    public void FadeTo(float targetAlpha) => FadeTo(targetAlpha, null);

    // Sobrecargas para código
    public void ShowFromCurrent(Action onComplete) => FadeTo(_visibleAlpha, onComplete);

    public void Show(Action onComplete)
    {
        SetAlpha(_hiddenAlpha);
        FadeTo(_visibleAlpha, onComplete);
    }

    public void HideFromCurrent(Action onComplete) => FadeTo(_hiddenAlpha, onComplete);

    public void Hide(Action onComplete)
    {
        SetAlpha(_visibleAlpha);
        FadeTo(_hiddenAlpha, onComplete);
    }

    public void SetVisible() => SetAlpha(_visibleAlpha);
    public void SetHidden() => SetAlpha(_hiddenAlpha);

    public void FadeTo(float targetAlpha, Action onComplete = null)
    {
        Initialize();

        KillTween();

        if (_duration <= 0f)
        {
            SetAlpha(targetAlpha);
            onComplete?.Invoke();
            return;
        }

        _alphaTween = DOTween
            .To(() => _currentAlpha, SetAlpha, Mathf.Clamp01(targetAlpha), _duration)
            .SetEase(_ease)
            .SetUpdate(_useUnscaledTime)
            .SetTarget(this)
            .OnComplete(() => onComplete?.Invoke());
    }

    public void SetAlpha(float alpha)
    {
        Initialize();

        _currentAlpha = Mathf.Clamp01(alpha);
        ApplyAlpha(_currentAlpha);
    }

    #region Internal

    protected virtual void Initialize() { }

    protected abstract void ApplyAlpha(float alpha);

    private void ApplyOnEnableBehaviour()
    {
        switch (_onEnableBehaviour)
        {
            case AlphaOnEnableBehaviour.ShowFromCurrent:
                ShowFromCurrent();
                break;
            case AlphaOnEnableBehaviour.HideFromCurrent:
                HideFromCurrent();
                break;
            case AlphaOnEnableBehaviour.Show:
                Show();
                break;
            case AlphaOnEnableBehaviour.Hide:
                Hide();
                break;
        }
    }

    private void KillTween()
    {
        if (_alphaTween == null) return;

        _alphaTween.Kill();
        _alphaTween = null;
    }

    #endregion
}
