using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InfoScreenController : MonoBehaviour
{
    [SerializeField] private Image _screenImage;
    [SerializeField] private TextMeshProUGUI _screenText;
    [SerializeField] private float _typingSpeed = 0.05f;

    private bool _isTyping;

    public bool IsFinished => !_isTyping;

    public void Initialize(ScreenDisplayData data)
    {
        if (_screenImage != null && data.DisplayImage != null)
        {
            _screenImage.sprite = data.DisplayImage;
        }

        if (_screenText != null && !string.IsNullOrEmpty(data.DisplayText))
        {
            StartCoroutine(TypeText(data.DisplayText));
        }
        else
        {
            _isTyping = false;
        }
    }

    private IEnumerator TypeText(string textToType)
    {
        _isTyping = true;
        _screenText.text = string.Empty;

        for (int i = 0; i < textToType.Length; i++)
        {
            _screenText.text += textToType[i];
            yield return new WaitForSeconds(_typingSpeed);
        }

        _isTyping = false;
    }

    public void SkipTyping(string fullText)
    {
        StopAllCoroutines();
        if (_screenText != null)
        {
            _screenText.text = fullText;
        }
        _isTyping = false;
    }
}