using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class SubmarineTestController : MonoBehaviour
{
    [Header("Configuración Física")]
    [SerializeField] private float _engineForce = 1500f;
    [SerializeField] private float _torqueForce = 500f;

    private Rigidbody _rb;

    private Vector2 _joystickMoveInput;
    private Vector2 _joystickRotationInput;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.mass = 500f;

        _rb.linearDamping = 2f;
        _rb.angularDamping = 2f;

        _rb.useGravity = false;
    }

    void FixedUpdate()
    {
        Keyboard keyboard = Keyboard.current;

        HandleMovement(keyboard);
        HandleRotation(keyboard);
    }

    private void HandleMovement(Keyboard keyboard)
    {
        Vector3 moveInput = new Vector3(_joystickMoveInput.x, 0f, _joystickMoveInput.y);

        if (keyboard != null)
        {
            if (keyboard.wKey.isPressed) moveInput.z += 1f;
            if (keyboard.sKey.isPressed) moveInput.z -= 1f;
            if (keyboard.dKey.isPressed) moveInput.x += 1f;
            if (keyboard.aKey.isPressed) moveInput.x -= 1f;
        }

        if (moveInput != Vector3.zero)
        {
            Vector3 forceDirection = transform.TransformDirection(moveInput.normalized);
            _rb.AddForce(forceDirection * _engineForce, ForceMode.Force);
        }
    }

    private void HandleRotation(Keyboard keyboard)
    {
        Vector3 torqueInput = new Vector3(_joystickRotationInput.y, _joystickRotationInput.x, 0f);

        if (keyboard != null)
        {
            if (keyboard.upArrowKey.isPressed) torqueInput.x -= 1f;
            if (keyboard.downArrowKey.isPressed) torqueInput.x += 1f;
            if (keyboard.rightArrowKey.isPressed) torqueInput.y += 1f;
            if (keyboard.leftArrowKey.isPressed) torqueInput.y -= 1f;

            if (keyboard.qKey.isPressed) torqueInput.z += 1f;
            if (keyboard.eKey.isPressed) torqueInput.z -= 1f;
        }

        if (torqueInput != Vector3.zero)
        {
            Vector3 localTorque = transform.TransformDirection(torqueInput.normalized);
            _rb.AddTorque(localTorque * _torqueForce, ForceMode.Force);
        }
    }

    // --- ENTRADAS PARA EL MOVIMIENTO (PLANO ZX) ---

    public void SetMoveX(float value)
    {
        _joystickMoveInput.x = value;
    }

    public void SetMoveZ(float value)
    {
        _joystickMoveInput.y = value;
    }

    // --- ENTRADAS PARA LA ROTACIÓN (EJES Y / X) ---

    public void SetRotationY(float value)
    {
        _joystickRotationInput.x = value;
    }

    public void SetRotationX(float value)
    {
        _joystickRotationInput.y = value;
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