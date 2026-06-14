using UnityEngine;

public abstract class MissionAreaBase : MonoBehaviour
{
    [SerializeField] private GameEvent _missionStartEvent;
    [SerializeField] private GameEvent _missionCompleteEvent;
    [Space(10)]
    [SerializeField] private LayerMask _submarineLayer = 8;
    [Space(10)]

    [SerializeField] private bool _drawAreaGizmo = true;
    [SerializeField] private Color _gizmoColor = new Color(1, 1, 0, 0.35f);
    [Space(10)]

    private bool _isActive = false;
    public bool IsActive => _isActive;

    private Collider _collider;


    private void OnTriggerEnter(Collider other)
    {
        if ((_submarineLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            OnSubmarineEntered(other);
        }
    }

    protected virtual void OnSubmarineEntered(Collider submarine)
    {
        if(_missionStartEvent != null) _missionStartEvent.Invoke();
        else
        {
            Debug.LogWarning($"MissionAreaBase on {gameObject.name} has no MissionStartEvent assigned.");
            return;
        }

        _collider.enabled = false;
    }

    protected virtual void FinishMission()
    {
        _missionCompleteEvent.Invoke();
        this.gameObject.SetActive(false);
    }

    protected virtual void OnEnable()
    {
        if(_missionCompleteEvent != null) _missionCompleteEvent.Subscribe(FinishMission);
        else
        {
            Debug.LogWarning($"MissionAreaBase on {gameObject.name} has no MissionCompleteEvent assigned.");
        }

        _collider = GetComponent<Collider>();
    }

    protected virtual void OnDisable()
    {
        if(_missionCompleteEvent != null) _missionCompleteEvent.Unsubscribe(FinishMission);
    }

    private void OnDrawGizmos()
    {
        if (!_drawAreaGizmo) return;

        Collider col = GetComponent<Collider>();

        if (col == null) return;

        Gizmos.color = _gizmoColor;
        Gizmos.matrix = transform.localToWorldMatrix;

        if (col is BoxCollider box)
        {
            Gizmos.DrawCube(box.center, box.size);
        }
        else if (col is SphereCollider sphere)
        {
            Gizmos.DrawSphere(sphere.center, sphere.radius);
        }
        else if (col is CapsuleCollider capsule)
        {
            Vector3 point1 = capsule.center + Vector3.up * (capsule.height / 2 - capsule.radius);
            Vector3 point2 = capsule.center - Vector3.up * (capsule.height / 2 - capsule.radius);
            Gizmos.DrawSphere(point1, capsule.radius);
            Gizmos.DrawSphere(point2, capsule.radius);
        }
    }
}
