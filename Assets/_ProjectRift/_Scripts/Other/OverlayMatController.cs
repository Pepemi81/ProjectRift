using UnityEngine;

public class OverlayMatController : OverlayController
{
    [Header("References")]
    [SerializeField] private Renderer _targetRenderer;

    [SerializeField] private string AlphaPropertyName = "_Alpha";

    private MaterialPropertyBlock _propertyBlock;
    private int _alphaPropertyId;

    private void Reset()
    {
        _targetRenderer = GetComponent<Renderer>();
    }

    private void OnValidate()
    {
        if (_targetRenderer == null)
        {
            _targetRenderer = GetComponent<Renderer>();
        }
    }

    #region Internal

    protected override void Initialize()
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

    protected override void ApplyAlpha(float alpha)
    {
        if (_targetRenderer == null) return;

        _targetRenderer.GetPropertyBlock(_propertyBlock);
        _propertyBlock.SetFloat(_alphaPropertyId, alpha);
        _targetRenderer.SetPropertyBlock(_propertyBlock);
    }

    #endregion
}
