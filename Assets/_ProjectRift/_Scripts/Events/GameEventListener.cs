using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class GameEventListener : MonoBehaviour
{
    [System.Serializable]
    public class DelayedAction
    {
        public float Delay;
        public UnityEvent Action;

        private WaitForSeconds _waitCache;

        public void Initialize()
        {
            if (Delay > 0f)
            {
                _waitCache = new WaitForSeconds(Delay);
            }
        }

        public IEnumerator Execute(MonoBehaviour runner)
        {
            if (Delay > 0f)
            {
                yield return _waitCache;
            }

            Action?.Invoke();
        }
    }

    [SerializeField] private GameEvent _gameEvent;
    [Space(10)]
    [SerializeField] private List<DelayedAction> _responses = new List<DelayedAction>();

    private void Awake()
    {
        foreach (var response in _responses)
        {
            response.Initialize();
        }
    }

    private void OnEnable()
    {
        if (_gameEvent != null)
        {
            _gameEvent.Subscribe(OnGameEventInvoked);
        }
    }

    private void OnDisable()
    {
        if (_gameEvent != null)
        {
            _gameEvent.Unsubscribe(OnGameEventInvoked);
        }
    }

    private void OnGameEventInvoked()
    {
        foreach (var response in _responses)
        {
            StartCoroutine(response.Execute(this));
        }
    }
}