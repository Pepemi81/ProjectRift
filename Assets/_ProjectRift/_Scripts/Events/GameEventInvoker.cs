using UnityEngine;

public class GameEventInvoker : MonoBehaviour
{
    [SerializeField] private GameEvent _gameEvent;

    [EditorButton("Invoke Event")]
    public void InvokeEvent()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning($"<color=Red>[GameEvent]</color> No se puede invocar {name} fuera de Play Mode.");
            return;
        }

        if (_gameEvent != null)
        {
            _gameEvent.Invoke();
        }
        else
        {
            Debug.LogWarning("<color=Red>[GameEventInvoker]</color> No GameEvent assigned to invoke.");
        }
    }
}
