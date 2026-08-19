using UnityEngine;

public enum ScreenDisplayMode
{
    DefaultUI,
    CustomPrefab
}

[CreateAssetMenu(menuName = "ScriptableObjects/Monitor/Slide Data")]
public class MonitorSlideData : ScriptableObject
{
    [SerializeField] private ScreenDisplayMode _displayMode;
    [SerializeField] private GameObject _slidePrefab;
    [SerializeField] private Sprite _displayImage;
    [TextArea(3, 10)][SerializeField] private string _displayText;

    [Tooltip("> 0 = Avanza automáticamente después de ese tiempo, 0 = Espera a que el jugador pulse un botón")]
    [SerializeField] private float _autoAdvanceDelay = 0f;

    [SerializeField] private MonitorSlideAudioSettings _audioSettings;
    public MonitorSlideAudioSettings AudioSettings => _audioSettings;
    public ScreenDisplayMode DisplayMode => _displayMode;
    public GameObject SlidePrefab => _slidePrefab;
    public Sprite DisplayImage => _displayImage;
    public string DisplayText => _displayText;
    public float AutoAdvanceDelay => _autoAdvanceDelay;
}