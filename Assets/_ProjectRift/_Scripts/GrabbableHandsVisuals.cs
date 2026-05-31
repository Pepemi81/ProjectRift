using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class GrabbableHandsVisuals : MonoBehaviour
{
    [SerializeField]private XRBaseInteractable _grabInteractable;

    [Header("PropHands")]
    [SerializeField] private GameObject _rightHandModel;
    [SerializeField] private GameObject _leftHandModel;


    private void Awake()
    {
        if (_rightHandModel != null) _rightHandModel.SetActive(false);
        if (_leftHandModel != null) _leftHandModel.SetActive(false);
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
                if (_rightHandModel == null) { Debug.LogError("PropRightHandModel no asignado."); return; }
                _rightHandModel.SetActive(true);
            }
            else if (hand.Side == HandSide.Left)
            {
                if (_leftHandModel == null) { Debug.LogError("PropLeftHandModel no asignado."); return; }
                _leftHandModel.SetActive(true);
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
                if (_rightHandModel != null) _rightHandModel.SetActive(false);
            }
            else if (hand.Side == HandSide.Left)
            {
                if (_leftHandModel != null) _leftHandModel.SetActive(false);
            }
        }
    }
}