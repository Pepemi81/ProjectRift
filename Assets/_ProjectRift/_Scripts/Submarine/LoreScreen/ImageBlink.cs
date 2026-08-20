using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Image))]
public class ImageBlink : MonoBehaviour
{
    private enum BlinkMode
    {
        Instant,
        Interpolated
    }

    [Header("References")]
    [SerializeField] private Image _image;

    [Header("Blink")]
    [SerializeField] private bool _playOnEnable = false;
    [SerializeField] private BlinkMode _blinkMode = BlinkMode.Instant;
    [SerializeField, Range(0f, 1f)] private float _visibleAlpha = 1f;
    [SerializeField, Range(0f, 1f)] private float _hiddenAlpha = 0f;
    [SerializeField, Min(0f)] private float _visibleDuration = 0.5f;
    [SerializeField, Min(0f)] private float _hiddenDuration = 0.5f;
    [SerializeField, Min(0f)] private float _transitionDuration = 0.15f;
    [SerializeField] private bool _restoreInitialAlphaOnDisable = true;

    private Coroutine _blinkCoroutine;
    private float _initialAlpha;

    private void Awake()
    {
        if (_image == null) _image = GetComponent<Image>();

        _initialAlpha = _image.color.a;
    }

    private void OnEnable()
    {
        if (_image == null) _image = GetComponent<Image>();

        _initialAlpha = _image.color.a;

        if (_playOnEnable) Blink();
    }

    private void OnDisable()
    {
        Stop();

        if (_restoreInitialAlphaOnDisable && _image != null)
        {
            SetAlpha(_initialAlpha);
        }
    }

    private void OnValidate()
    {
        if (_image == null)
        {
            _image = GetComponent<Image>();
        }
    }

    [EditorButton("Blink")]
    public void Blink()
    {
        if (!isActiveAndEnabled || _image == null)
        {
            return;
        }

        Stop();
        _blinkCoroutine = StartCoroutine(BlinkRoutine());
    }

    [EditorButton("Stop")]
    public void Stop()
    {
        if (_blinkCoroutine != null)
        {
            StopCoroutine(_blinkCoroutine);
            _blinkCoroutine = null;
        }
    }

    public void SetVisible()
    {
        Stop();
        SetAlpha(_visibleAlpha);
    }

    public void SetHidden()
    {
        Stop();
        SetAlpha(_hiddenAlpha);
    }

    private IEnumerator BlinkRoutine()
    {
        while (true)
        {
            yield return SetBlinkAlpha(_visibleAlpha);
            yield return Wait(_visibleDuration);

            yield return SetBlinkAlpha(_hiddenAlpha);
            yield return Wait(_hiddenDuration);
        }
    }

    private IEnumerator SetBlinkAlpha(float targetAlpha)
    {
        if (_blinkMode == BlinkMode.Instant || _transitionDuration <= 0f)
        {
            SetAlpha(targetAlpha);
            yield break;
        }

        float startAlpha = _image.color.a;
        float timer = 0f;

        while (timer < _transitionDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / _transitionDuration);
            SetAlpha(Mathf.Lerp(startAlpha, targetAlpha, t));
            yield return null;
        }

        SetAlpha(targetAlpha);
    }

    private IEnumerator Wait(float duration)
    {
        if (duration <= 0f) yield break;

        yield return new WaitForSeconds(duration);
    }

    private void SetAlpha(float alpha)
    {
        Color color = _image.color;
        color.a = alpha;
        _image.color = color;
    }
}
