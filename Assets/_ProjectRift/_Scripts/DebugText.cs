using TMPro;
using UnityEngine;
using System.Collections;

public class DebugText : Singleton<DebugText>
{
    [SerializeField] private TextMeshPro _text;

    private Coroutine _clearCoroutine;

    public void SetText(string text, float duration = 0f)
    {
        if (_clearCoroutine != null) StopCoroutine(_clearCoroutine);

        _text.text = text;

        if (duration > 0f)
        {
            _clearCoroutine = StartCoroutine(ClearTextAfterDelay(duration));
        }
    }

    private IEnumerator ClearTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        _text.text = string.Empty;
    }
}