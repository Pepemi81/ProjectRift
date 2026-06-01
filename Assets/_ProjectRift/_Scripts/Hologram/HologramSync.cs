using UnityEngine;

public class HologramSync : Singleton<HologramSync>
{
    [SerializeField] private Transform _realSubmarine;
    [SerializeField] private Transform _hologramOrigin;
    public Transform HologramOrigin => _hologramOrigin;

    [Header("Escala")]
    [SerializeField] private float _scaleFactor = 0.01f;

    private void LateUpdate()
    {
        if (_realSubmarine == null || _hologramOrigin == null) return;

        Quaternion inverseRotation = Quaternion.Inverse(_realSubmarine.rotation);
        _hologramOrigin.localRotation = inverseRotation;

        Vector3 scaledPosition = _realSubmarine.position * _scaleFactor;
        _hologramOrigin.localPosition = inverseRotation * (-scaledPosition);
    }
}