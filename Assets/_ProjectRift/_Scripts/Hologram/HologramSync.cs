using UnityEngine;

public class HologramSync : Singleton<HologramSync>
{
    [SerializeField] private Transform _realSubmarine;
    [SerializeField] private Transform _hologramSubmarine;
    [SerializeField] private Transform _hologramOrigin;
    public Transform HologramOrigin => _hologramOrigin;

    [Header("Escala")]
    [SerializeField] private float _scaleFactor = 0.01f;
    [SerializeField] private float _minScale = 0.005f;
    [SerializeField] private float _maxScale = 0.05f;

    private void LateUpdate()
    {
        if (_realSubmarine == null || _hologramOrigin == null) return;

        _hologramOrigin.localScale = Vector3.one * _scaleFactor;
        _hologramOrigin.localScale = Vector3.one * _scaleFactor;

        Quaternion inverseRotation = Quaternion.Inverse(_realSubmarine.rotation);
        _hologramOrigin.localRotation = inverseRotation;

        Vector3 scaledPosition = _realSubmarine.position * _scaleFactor;
        _hologramOrigin.localPosition = inverseRotation * (-scaledPosition);
    }

    public void SetScaleFactor(float _scaleFactor)
    {
        _scaleFactor = Mathf.Clamp(_scaleFactor, _minScale, _maxScale);
    }
}