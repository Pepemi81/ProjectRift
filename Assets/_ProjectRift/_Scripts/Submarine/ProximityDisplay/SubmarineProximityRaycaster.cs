using UnityEngine;

public enum ProximityAxis
{
    Forward,
    Backward,
    Right,
    Left,
    Up,
    Down
}

public class SubmarineProximityRaycaster : MonoBehaviour
{
    [Header("Rays")]
    [SerializeField, Min(0f)] private float _raycastLength = 5f;
    [SerializeField] private LayerMask _collisionMask = ~0;
    private readonly float[] _values = new float[6];


    [Space, Header("Debug")]
    [SerializeField] private bool _showDebugRay;
    [SerializeField] private Color _raycastColor = Color.cyan;
    [SerializeField] private Color _hitColor = Color.red;

    public float Forward => GetValue(ProximityAxis.Forward);
    public float Backward => GetValue(ProximityAxis.Backward);
    public float Right => GetValue(ProximityAxis.Right);
    public float Left => GetValue(ProximityAxis.Left);
    public float Up => GetValue(ProximityAxis.Up);
    public float Down => GetValue(ProximityAxis.Down);

    private void Awake()
    {
        TickRay();
    }

    private void FixedUpdate()
    {
        TickRay();
    }

    public void TickRay()
    {
        Cast(ProximityAxis.Forward, transform.forward);
        Cast(ProximityAxis.Backward, -transform.forward);
        Cast(ProximityAxis.Right, transform.right);
        Cast(ProximityAxis.Left, -transform.right);
        Cast(ProximityAxis.Up, transform.up);
        Cast(ProximityAxis.Down, -transform.up);
    }

    public float GetValue(ProximityAxis axis)
    {
        return _values[(int)axis];
    }

    private void Cast(ProximityAxis axis, Vector3 direction)
    {
        int index = (int)axis;

        bool hasHit = Physics.Raycast(
            transform.position,
            direction,
            out RaycastHit hit,
            _raycastLength,
            _collisionMask);

        _values[index] = hasHit ? Mathf.Clamp01(hit.distance / _raycastLength) : 1f;
    }

    private void OnDrawGizmosSelected()
    {
        if (!_showDebugRay)
        {
            return;
        }

        DrawDebugRay(ProximityAxis.Forward, transform.forward);
        DrawDebugRay(ProximityAxis.Backward, -transform.forward);
        DrawDebugRay(ProximityAxis.Right, transform.right);
        DrawDebugRay(ProximityAxis.Left, -transform.right);
        DrawDebugRay(ProximityAxis.Up, transform.up);
        DrawDebugRay(ProximityAxis.Down, -transform.up);
    }

    private void DrawDebugRay(ProximityAxis axis, Vector3 direction)
    {
        float value = Application.isPlaying ? GetValue(axis) : 1f;
        bool hasHit = value < 1f;
        float length = value * _raycastLength;

        Gizmos.color = hasHit ? _hitColor : _raycastColor;
        Gizmos.DrawRay(transform.position, direction * length);

        if (hasHit)
        {
            Gizmos.DrawSphere(transform.position + direction * length, 0.05f);
        }
    }
}
