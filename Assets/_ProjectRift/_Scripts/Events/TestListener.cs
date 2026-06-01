using UnityEngine;

public class TestListener : MonoBehaviour
{
    [Header("Event to listen")]
    [SerializeField] private GameEvent gameEvent;


    private void OnEnable()
    {
        gameEvent?.Subscribe(PerformAction);
    }

    private void OnDisable()
    {
        gameEvent?.Unsubscribe(PerformAction);
    }

    private void PerformAction()
    {
        Debug.Log($"<color=Green>Evento con el nombre {gameEvent} recibido!</color>");
    }
}
