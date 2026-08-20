using UnityEngine;

public class ProximitySegmentVisual : MonoBehaviour
{
    [Header("Renderer")]
    [SerializeField] private Renderer _renderer;
    [SerializeField] private string _colorProperty = "_BaseColor";
    [SerializeField] private string _emissionProperty = "_EmissionColor";

    [Header("Light")]
    [SerializeField] private Light _light;
    [SerializeField, Min(0f)] private float _lightOnIntensity = 1f;

    [Header("State")]
    [SerializeField] private Color _offColor = Color.black;
    [SerializeField] private Color _onColor = Color.red;
    [SerializeField, Min(0f)] private float _offEmission = 0f;
    [SerializeField, Min(0f)] private float _onEmission = 1.5f;

    private MaterialPropertyBlock _propertyBlock;

    private void Awake()
    {
        Initialize();
        SetState(false);
    }

    private void Reset()
    {
        _renderer = GetComponent<Renderer>();
        _light = GetComponentInChildren<Light>();
    }

    private void OnValidate()
    {
        if (_renderer == null) _renderer = GetComponent<Renderer>();
    }

    public void SetState(bool isOn)
    {
        Initialize();

        Color color = isOn ? _onColor : _offColor;
        float emission = isOn ? _onEmission : _offEmission;

        if (_renderer != null)
        {
            _renderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetColor(_colorProperty, color);
            _propertyBlock.SetColor(_emissionProperty, color * emission);
            _renderer.SetPropertyBlock(_propertyBlock);
        }

        if (_light != null)
        {
            _light.enabled = isOn;
            _light.color = _onColor;
            _light.intensity = isOn ? _lightOnIntensity : 0f;
        }
    }

    private void Initialize()
    {
        if (_propertyBlock == null)
        {
            _propertyBlock = new MaterialPropertyBlock();
        }
    }
}
