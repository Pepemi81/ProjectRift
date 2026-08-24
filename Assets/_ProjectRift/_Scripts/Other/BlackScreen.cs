using UnityEngine;
using UnityEngine.SceneManagement;

public class BlackScreen : MonoBehaviour
{
    [SerializeField] private Transform _targetTransform;
    [SerializeField, Space] private Vector3 _distanceOffset;
    [SerializeField, Space] private Vector3 _angleOffset;
    [SerializeField, Space] private bool _isTracking;
    [SerializeField] private bool _searchTargetOnEnable;
    [SerializeField] private bool _dontDestroyOnLoad;

    private void OnEnable()
    {
        if (_dontDestroyOnLoad) DontDestroyOnLoad(this.gameObject);

        FindTarget();

        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void OnDisable() => SceneManager.sceneLoaded -= HandleSceneLoaded;

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode) => FindTarget();

    private void FindTarget()
    {
        if (!_searchTargetOnEnable) return;

        if (Camera.main != null)
        {
            _targetTransform = Camera.main.transform;
        }
        else
        {
            Debug.LogWarning("<color=orange>[BlackScreen]</color> No se encontró Camera.main en la escena cargada.");
        }
    }

    private void LateUpdate()
    {
        if (!_isTracking || _targetTransform == null) return;

        transform.SetPositionAndRotation(
            _targetTransform.TransformPoint(_distanceOffset),
            _targetTransform.rotation * Quaternion.Euler(_angleOffset)
        );
    }
}
