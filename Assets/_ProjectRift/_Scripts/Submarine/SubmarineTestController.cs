using UnityEngine;
using UnityEngine.InputSystem;

public class SubmarineTestController : MonoBehaviour
{
    [Header("Configuración Física")]
    [SerializeField] private float _engineForce = 1500f;
    [SerializeField] private float _torqueForce = 500f;

    private Rigidbody _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.mass = 500f;

        _rb.linearDamping = 2f;
        _rb.angularDamping = 2f;

        _rb.useGravity = false;
    }

    void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;

        HandleMovement(keyboard);
        HandleRotation(keyboard);
    }

    private void HandleMovement(Keyboard keyboard)
    {
        Vector3 moveInput = Vector3.zero;

        if (keyboard.wKey.isPressed) moveInput.z += 1f;
        if (keyboard.sKey.isPressed) moveInput.z -= 1f;
        if (keyboard.dKey.isPressed) moveInput.x += 1f;
        if (keyboard.aKey.isPressed) moveInput.x -= 1f;

        if (moveInput != Vector3.zero)
        {
            Vector3 forceDirection = transform.TransformDirection(moveInput.normalized);
            _rb.AddForce(forceDirection * _engineForce, ForceMode.Force);
        }
    }

    private void HandleRotation(Keyboard keyboard)
    {
        Vector3 torqueInput = Vector3.zero;

        if (keyboard.upArrowKey.isPressed) torqueInput.x -= 1f;
        if (keyboard.downArrowKey.isPressed) torqueInput.x += 1f;
        if (keyboard.rightArrowKey.isPressed) torqueInput.y += 1f;
        if (keyboard.leftArrowKey.isPressed) torqueInput.y -= 1f;

        if (keyboard.qKey.isPressed) torqueInput.z += 1f;
        if (keyboard.eKey.isPressed) torqueInput.z -= 1f;

        if (torqueInput != Vector3.zero)
        {
            Vector3 localTorque = transform.TransformDirection(torqueInput.normalized);
            _rb.AddTorque(localTorque * _torqueForce, ForceMode.Force);
        }
    }

    [ContextMenu("Reset Inercia Submarino")]
    public void ResetVelocity()
    {
        if (_rb != null)
        {
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            Debug.LogWarning("Inercias del submarino reseteadas por seguridad.");
        }
    }
}