using UnityEngine;

public class BlackScreen : MonoBehaviour
{
    [SerializeField] private Transform _trackedTransform;
    [SerializeField, Space] private Vector3 _offset;
    [SerializeField, Space] private bool _isTracking;
    [SerializeField] private bool _searchMainCam;

    private void OnEnable()
    {
        if (!_searchMainCam) return;

        _trackedTransform = Camera.main.transform;
    }


    private void Update()
    {
        if (!_isTracking) return;

        transform.position = _trackedTransform.position + _offset;
    }
}
