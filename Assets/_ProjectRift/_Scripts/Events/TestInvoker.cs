using UnityEngine;

public class TestInvoker : MonoBehaviour
{
    [Header("Event to Invoke")]
    [SerializeField] private GameEvent gameEvent;

    [ContextMenu("Invoke Event")]
    private void InvokeEvent()
    {
        gameEvent?.Invoke();
    }
}
