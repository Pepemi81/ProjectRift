using DG.Tweening;
using TMPro;
using UnityEngine;

public class OverlayTextController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text _targetText;

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

    private void Reset()
    {
        _targetText = GetComponent<TMP_Text>();
    }

    private void Awake()
    {
        Initialize();
    }

    private void OnEnable()
    {
        Initialize();
        ApplyOnEnableBehaviour();
    }

    private void OnDisable()
    {
        KillTween();
    }

    private void OnValidate()
    {
        if (_targetText == null)
        {
            _targetText = GetComponent<TMP_Text>();
        }
    }

    public void Show()
    {
        FadeTo(_visibleAlpha);
    }

    public void Hide()
    {
        FadeTo(_hiddenAlpha);
    }

    public void SetVisible()
    {
        SetAlpha(_visibleAlpha);
    }

    public void SetHidden()
    {
        SetAlpha(_hiddenAlpha);
    }

    public void FadeTo(float targetAlpha)
    {
        Initialize();

        if (_targetText == null) return;

        KillTween();

        if (_duration <= 0f)
        {
            SetAlpha(targetAlpha);
            return;
        }

        _alphaTween = DOTween
            .To(() => _currentAlpha, SetAlpha, Mathf.Clamp01(targetAlpha), _duration)
            .SetEase(_ease)
            .SetUpdate(_useUnscaledTime)
            .SetTarget(this);
    }

    public void SetAlpha(float alpha)
    {
        Initialize();

        if (_targetText == null) return;

        _currentAlpha = Mathf.Clamp01(alpha);

        Color color = _targetText.color;
        color.a = _currentAlpha;
        _targetText.color = color;
    }

    #region Internal

    private void Initialize()
    {
        if (_targetText == null)
        {
            _targetText = GetComponent<TMP_Text>();
        }
    }

    private void ApplyOnEnableBehaviour()
    {
        switch (_onEnableBehaviour)
        {
            case AlphaOnEnableBehaviour.ShowFromCurrent:
                Show();
                break;
            case AlphaOnEnableBehaviour.HideFromCurrent:
                Hide();
                break;
            case AlphaOnEnableBehaviour.Show:
                SetAlpha(_hiddenAlpha);
                Show();
                break;
            case AlphaOnEnableBehaviour.Hide:
                SetAlpha(_visibleAlpha);
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
