using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "ScriptableObjects/GameEvent")]
public class GameEvent : ScriptableObject
{
    private UnityAction _onEventTriggered;

    [ContextMenu("Invoke Event")]
    public void Invoke() => _onEventTriggered?.Invoke();

    public void Subscribe(UnityAction action) => _onEventTriggered += action;

    public void Unsubscribe(UnityAction action) => _onEventTriggered -= action;
}