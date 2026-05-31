using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class GrabbableHandToggler : MonoBehaviour
{
    [SerializeField]private XRBaseInteractable _grabInteractable;

    [Header("PropHands")]
    [SerializeField] private GameObject _propRightHandModel;
    [SerializeField] private GameObject _propLeftHandModel;


    private void Awake()
    {
        if (_propRightHandModel != null) _propRightHandModel.SetActive(false);
        if (_propLeftHandModel != null) _propLeftHandModel.SetActive(false);
    }

    private void OnEnable()
    {
        _grabInteractable.selectEntered.AddListener(OnGrabbed);
        _grabInteractable.selectExited.AddListener(OnReleased);
    }

    private void OnDisable()
    {
        _grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        _grabInteractable.selectExited.RemoveListener(OnReleased);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        IXRSelectInteractor interactor = args.interactorObject;
        HandAnimator hand = interactor.transform.GetComponent<HandAnimator>();

        if (hand != null)
        {
            hand.ToggleVisibility(false);

            if (hand.Side == HandSide.Right)
            {
                if (_propRightHandModel == null) { Debug.LogError("PropRightHandModel no asignado."); return; }
                _propRightHandModel.SetActive(true);
            }
            else if (hand.Side == HandSide.Left)
            {
                if (_propLeftHandModel == null) { Debug.LogError("PropLeftHandModel no asignado."); return; }
                _propLeftHandModel.SetActive(true);
            }
        }
        else
        {
            Debug.LogError("No se encontró un componente HandAnimator en el interactor.");
        }
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        IXRSelectInteractor interactor = args.interactorObject;
        HandAnimator hand = interactor.transform.GetComponent<HandAnimator>();

        if (hand != null)
        {
            hand.ToggleVisibility(true);

            if (hand.Side == HandSide.Right)
            {
                if (_propRightHandModel != null) _propRightHandModel.SetActive(false);
            }
            else if (hand.Side == HandSide.Left)
            {
                if (_propLeftHandModel != null) _propLeftHandModel.SetActive(false);
            }
        }
    }
}