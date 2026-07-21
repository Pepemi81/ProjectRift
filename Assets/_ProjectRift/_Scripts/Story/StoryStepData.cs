using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;

[System.Serializable]
public class StoryStepData
{
    [SerializeField] private string _stepName;
    [SerializeField, TextArea(2, 5)] private string _notes;

    [Header("Start")]
    [SerializeField] private GameEvent[] _eventsOnStart;

    [Header("Game Event Wait")]
    [SerializeField] private GameEvent _waitForEvent;

    [Header("Timeline")]
    [SerializeField] private PlayableAsset _timelineAsset;
    [SerializeField] private bool _waitForTimelineToFinish = true;

    [Space(15)]
    [Tooltip("Tiempo a esperar después de que se cumplan el timeline y/o el evento")]
    [SerializeField, Min(0f)] private float _delayAfterConditions;

    [Header("Finish")]
    [SerializeField] private GameEvent[] _eventsOnComplete;

    public string StepName => _stepName;
    public string Notes => _notes;
    public GameEvent[] EventsOnStart => _eventsOnStart;
    public PlayableAsset TimelineAsset => _timelineAsset;
    public bool WaitForTimelineToFinish => _waitForTimelineToFinish;
    public GameEvent WaitForEvent => _waitForEvent;
    public float DelayAfterConditions => _delayAfterConditions;
    public GameEvent[] EventsOnComplete => _eventsOnComplete;
}
