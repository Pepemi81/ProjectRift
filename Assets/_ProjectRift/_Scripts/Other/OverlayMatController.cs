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

public class OverlayMatController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Renderer _targetRenderer;

    [Header("Alpha")]
    [SerializeField] private string AlphaPropertyName = "_Alpha";
    [SerializeField, Range(0f, 1f)] private float _hiddenAlpha;
    [SerializeField, Range(0f, 1f)] private float _visibleAlpha = 1f;

    [Header("Animation")]
    [SerializeField] private AlphaOnEnableBehaviour _onEnableBehaviour = AlphaOnEnableBehaviour.Show;
    [SerializeField, Min(0f)] private float _duration = 3f;
    [SerializeField] private Ease _ease = Ease.InOutQuad;
    [SerializeField] private bool _useUnscaledTime;

    private MaterialPropertyBlock _propertyBlock;
    private Tween _alphaTween;
    private int _alphaPropertyId;
    private float _currentAlpha;

    public float CurrentAlpha => _currentAlpha;
    public bool IsAnimating => _alphaTween != null && _alphaTween.IsActive() && _alphaTween.IsPlaying();

    private void Reset()
    {
        _targetRenderer = GetComponent<Renderer>();
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
        if (_targetRenderer == null)
        {
            _targetRenderer = GetComponent<Renderer>();
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

        if (_targetRenderer == null) return;

        _currentAlpha = Mathf.Clamp01(alpha);

        _targetRenderer.GetPropertyBlock(_propertyBlock);
        _propertyBlock.SetFloat(_alphaPropertyId, _currentAlpha);
        _targetRenderer.SetPropertyBlock(_propertyBlock);
    }

    #region Internal

    private void Initialize()
    {
        if (_targetRenderer == null)
        {
            _targetRenderer = GetComponent<Renderer>();
        }

        if (_propertyBlock == null)
        {
            _propertyBlock = new MaterialPropertyBlock();
        }

        if (_alphaPropertyId == 0)
        {
            _alphaPropertyId = Shader.PropertyToID(AlphaPropertyName);
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
