using TMPro;
using UnityEngine;

public class OverlayTextController : OverlayController
{
    [Header("References")]
    [SerializeField] private TMP_Text _targetText;

    private void Reset()
    {
        _targetText = GetComponent<TMP_Text>();
    }

    private void OnValidate()
    {
        if (_targetText == null)
        {
            _targetText = GetComponent<TMP_Text>();
        }
    }

    #region Internal

    protected override void Initialize()
    {
        if (_targetText == null)
        {
            _targetText = GetComponent<TMP_Text>();
        }
    }

    protected override void ApplyAlpha(float alpha)
    {
        if (_targetText == null) return;

        Color color = _targetText.color;
        color.a = alpha;
        _targetText.color = color;

        Color32 faceColor = _targetText.faceColor;
        faceColor.a = (byte)Mathf.RoundToInt(alpha * 255f);
        _targetText.faceColor = faceColor;
    }

    #endregion
}
