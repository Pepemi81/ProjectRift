using UnityEngine;

public class HologramSync : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform _realElement;
    [SerializeField] private Transform _hologramElement;

    [Header("Escala")]
    [SerializeField] private float _scaleFactor = 0.1f;

    private void LateUpdate()
    {
        if (_realElement == null || _hologramElement == null) return;

        Vector3 localPos = _realElement.position;
        _hologramElement.localPosition = localPos * _scaleFactor;

        _hologramElement.localRotation = _realElement.rotation;
    }
}