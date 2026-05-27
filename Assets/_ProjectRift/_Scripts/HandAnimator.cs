using UnityEngine;
using UnityEngine.InputSystem;

public class HandAnimator : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Animator _handAnimator;

    [Header("Configuración de Inputs (OpenXR)")]
    [SerializeField] private InputActionProperty _gripAction;
    [SerializeField] private InputActionProperty _triggerAction;
    [SerializeField] private float _animationSpeed = 0.5f;

    private int _gripParamId;
    private int _triggerParamId;

    private float _currentGrip;
    private float _currentTrigger;

    private void Start()
    {
        if (_handAnimator == null)
        {
            _handAnimator = GetComponent<Animator>();
        }

        _gripParamId = Animator.StringToHash("Grip");
        _triggerParamId = Animator.StringToHash("Trigger");
    }

    private void Update()
    {
        AnimateHand();
    }

    private void AnimateHand()
    {
        if (_gripAction.action == null || _triggerAction.action == null) return;

        float targetGrip = _gripAction.action.ReadValue<float>();
        float targetTrigger = _triggerAction.action.ReadValue<float>();

        _currentGrip = Mathf.MoveTowards(_currentGrip, targetGrip, _animationSpeed * Time.deltaTime);
        _currentTrigger = Mathf.MoveTowards(_currentTrigger, targetTrigger, _animationSpeed * Time.deltaTime);

        _handAnimator.SetFloat(_gripParamId, _currentGrip);
        _handAnimator.SetFloat(_triggerParamId, _currentTrigger);
    }
}