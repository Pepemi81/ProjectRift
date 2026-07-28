using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Define el modo de revelado del texto en prefabs personalizados.
/// </summary>
public enum TypingMode
{
    Sequential,
    Simultaneous
}

/// <summary>
/// Se coloca en la raíz de un prefab de interfaz personalizado para auto-gestionar 
/// la animación de escritura en sus componentes TextMeshProUGUI.
/// </summary>
public class CustomSlideAnimator : MonoBehaviour
{
    [SerializeField] private List<TextMeshProUGUI> _textElements;
    [SerializeField] private TypingMode _typingMode;
    [SerializeField] private float _typingSpeed = 0.05f;

    private string[] _originalTexts;
    private bool _isTyping;

    public bool IsFinished => !_isTyping;

    public void Initialize()
    {
        _originalTexts = new string[_textElements.Count];

        for (int i = 0; i < _textElements.Count; i++)
        {
            if (_textElements[i] != null)
            {
                _originalTexts[i] = _textElements[i].text;
                _textElements[i].text = string.Empty;
            }
        }

        if (_textElements.Count > 0)
        {
            if (_typingMode == TypingMode.Simultaneous)
            {
                StartCoroutine(TypeSimultaneously());
            }
            else
            {
                StartCoroutine(TypeSequentially());
            }
        }
        else
        {
            _isTyping = false;
        }
    }

    private IEnumerator TypeSimultaneously()
    {
        _isTyping = true;
        int maxLength = 0;

        foreach (string text in _originalTexts)
        {
            if (text != null && text.Length > maxLength)
            {
                maxLength = text.Length;
            }
        }

        for (int i = 0; i < maxLength; i++)
        {
            for (int j = 0; j < _textElements.Count; j++)
            {
                if (_textElements[j] != null && _originalTexts[j] != null && i < _originalTexts[j].Length)
                {
                    _textElements[j].text += _originalTexts[j][i];
                }
            }
            yield return new WaitForSeconds(_typingSpeed);
        }

        _isTyping = false;
    }

    private IEnumerator TypeSequentially()
    {
        _isTyping = true;

        for (int i = 0; i < _textElements.Count; i++)
        {
            if (_textElements[i] == null || string.IsNullOrEmpty(_originalTexts[i])) continue;

            string fullText = _originalTexts[i];

            for (int j = 0; j < fullText.Length; j++)
            {
                _textElements[i].text += fullText[j];
                yield return new WaitForSeconds(_typingSpeed);
            }
        }

        _isTyping = false;
    }

    public void SkipTyping()
    {
        StopAllCoroutines();

        for (int i = 0; i < _textElements.Count; i++)
        {
            if (_textElements[i] != null && _originalTexts != null && i < _originalTexts.Length)
            {
                _textElements[i].text = _originalTexts[i];
            }
        }

        _isTyping = false;
    }
}