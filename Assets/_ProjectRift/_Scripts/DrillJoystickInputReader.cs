using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class DrillJoystickInputReader : MonoBehaviour
{
    [Header("Asignación de Inputs")]
    [SerializeField] private InputActionProperty _leftJoystickAction;
    [SerializeField] private InputActionProperty _rightJoystickAction;
    [SerializeField] private InputActionProperty _leftTriggerAction;
    [SerializeField] private InputActionProperty _rightTriggerAction;

    private Coroutine _drillCoroutine;
    private HandSide _grabbedSide;
    private XRGrabInteractable _grabInteractable;

    private void Awake() => _grabInteractable = GetComponent<XRGrabInteractable>();

    private void OnEnable()
    {
        _leftJoystickAction.action?.Enable();
        _rightJoystickAction.action?.Enable();
        _leftTriggerAction.action?.Enable();
        _rightTriggerAction.action?.Enable();

        _grabInteractable.selectEntered.AddListener(StartDrilling);
        _grabInteractable.selectExited.AddListener(StopDrilling);
    }

    private void StartDrilling(SelectEnterEventArgs args)
    {
        HandAnimator hand = args.interactorObject.transform.GetComponent<HandAnimator>();
        if (hand != null) _grabbedSide = hand.Side;

        if (_drillCoroutine != null) StopCoroutine(_drillCoroutine);
        _drillCoroutine = StartCoroutine(DrillUpdate());
    }

    private void StopDrilling(SelectExitEventArgs args)
    {
        if (_drillCoroutine != null)
        {
            StopCoroutine(_drillCoroutine);
            _drillCoroutine = null;
        }
    }

    private IEnumerator DrillUpdate()
    {
        while (true)
        {
            Vector2 stickValue = (_grabbedSide == HandSide.Right) ?
                _rightJoystickAction.action.ReadValue<Vector2>() :
                _leftJoystickAction.action.ReadValue<Vector2>();

            float triggerValue = (_grabbedSide == HandSide.Right) ?
                _rightTriggerAction.action.ReadValue<float>() :
                _leftTriggerAction.action.ReadValue<float>();

            ProcessDrillInput(stickValue, triggerValue);

            yield return null;
        }
    }

    private void ProcessDrillInput(Vector2 stick, float trigger)
    {
        DebugText.Instance.SetText($"<color=green>Joystick: {stick}</color>\n<color=blue>Trigger: {trigger}</color>");
    }
}