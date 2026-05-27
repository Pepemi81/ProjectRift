using UnityEngine;
using TMPro;

public class DebugText : Singleton<DebugText>
{
    [SerializeField] private TextMeshPro _text;

    public void SetText(string text)
    {
        if (_text != null)
        {
            _text.text = text;
        }
    }
}
