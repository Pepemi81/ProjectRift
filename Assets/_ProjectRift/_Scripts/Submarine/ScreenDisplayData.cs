using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Monitor/Screen Data")]
public class ScreenDisplayData : ScriptableObject
{
    [SerializeField] private GameObject _screenPrefab;
    [SerializeField] private Sprite _displayImage;
    [TextArea(3, 10)] [SerializeField] private string _displayText;
    
    // Si es > 0, avanza automáticamente al pasar ese tiempo.
    // Si es 0, se quedará esperando a que el jugador pulse un botón.
    [SerializeField] private float _autoAdvanceDelay = 0f;

    public GameObject ScreenPrefab => _screenPrefab;
    public Sprite DisplayImage => _displayImage;
    public string DisplayText => _displayText;
    public float AutoAdvanceDelay => _autoAdvanceDelay;
}

[CreateAssetMenu(menuName = "Monitor/Screen Sequence")]
public class ScreenSequenceData : ScriptableObject
{
    [SerializeField] private List<ScreenDisplayData> _screens = new List<ScreenDisplayData>();
    
    public List<ScreenDisplayData> Screens => _screens;
}