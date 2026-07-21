using UnityEngine;
using System.Collections;
using UnityEngine.Playables;

public class StoryManager : Singleton<StoryManager>
{
    [Header("Chapter")]
    [SerializeField] private StoryChapterData _currentChapter;
    [SerializeField] private bool _playOnStart = true;

    [Header("Timeline")]
    [SerializeField] private PlayableDirector _timelineDirector;

    [Header("Debug")]
    [SerializeField] private bool _logProgress = true;

    private Coroutine _chapterCoroutine;
    private bool _waitingEventReceived;
    private GameEvent _activeWaitEvent;

    public StoryChapterData CurrentChapter => _currentChapter;
    public int CurrentStepIndex { get; private set; } = -1;
    public bool IsPlaying => _chapterCoroutine != null;

    private void Start()
    {
        if (_playOnStart)
        {
            PlayChapter(_currentChapter);
        }
    }

    public void PlayChapter(StoryChapterData chapter)
    {
        if (chapter == null)
        {
            Debug.LogWarning("<color=orange>[StoryManager]</color> Cannot play a null chapter.");
            return;
        }

        StopCurrentChapter();
        _currentChapter = chapter;
        _chapterCoroutine = StartCoroutine(ProcessChapter(chapter));
    }

    public void StopCurrentChapter()
    {
        if (_chapterCoroutine != null)
        {
            StopCoroutine(_chapterCoroutine);
            _chapterCoroutine = null;
        }

        UnsubscribeFromActiveWaitEvent();
        CurrentStepIndex = -1;
        _waitingEventReceived = false;
    }

    private void OnDisable()
    {
        StopCurrentChapter();
    }

    private IEnumerator ProcessChapter(StoryChapterData chapter)
    {
        Log($"Starting chapter: {chapter.ChapterName}");

        for (int i = 0; i < chapter.Steps.Count; i++)
        {
            CurrentStepIndex = i;
            yield return ProcessStep(chapter.Steps[i]);
        }

        Log($"Chapter finished: {chapter.ChapterName}");
        CurrentStepIndex = -1;
        _chapterCoroutine = null;
    }

    private IEnumerator ProcessStep(StoryStepData step)
    {
        if (step == null)
        {
            yield break;
        }

        Log($"Step started: {step.StepName}");

        SubscribeToWaitEvent(step.WaitForEvent);
        InvokeEvents(step.EventsOnStart);

        if (step.TimelineAsset != null)
        {
            yield return PlayTimeline(step);
        }

        if (step.WaitForEvent != null)
        {
            yield return new WaitUntil(() => _waitingEventReceived);
        }

        UnsubscribeFromActiveWaitEvent();

        if (step.DelayAfterConditions > 0f)
        {
            yield return new WaitForSeconds(step.DelayAfterConditions);
        }

        InvokeEvents(step.EventsOnComplete);

        Log($"Step completed: {step.StepName}");
    }

    private IEnumerator PlayTimeline(StoryStepData step)
    {
        if (_timelineDirector == null)
        {
            Debug.LogWarning($"<color=orange>[StoryManager]</color> Step '{step.StepName}' has a Timeline Asset but no PlayableDirector assigned.");
            yield break;
        }

        _timelineDirector.playableAsset = step.TimelineAsset;
        _timelineDirector.Play();

        if (!step.WaitForTimelineToFinish)
        {
            yield break;
        }

        while (_timelineDirector.state == PlayState.Playing)
        {
            yield return null;
        }
    }

    private void SubscribeToWaitEvent(GameEvent gameEvent)
    {
        UnsubscribeFromActiveWaitEvent();
        _waitingEventReceived = false;

        if (gameEvent == null)
        {
            return;
        }

        _activeWaitEvent = gameEvent;
        _activeWaitEvent.Subscribe(OnWaitEventReceived);
    }

    private void UnsubscribeFromActiveWaitEvent()
    {
        if (_activeWaitEvent != null)
        {
            _activeWaitEvent.Unsubscribe(OnWaitEventReceived);
            _activeWaitEvent = null;
        }

        _waitingEventReceived = false;
    }

    private void OnWaitEventReceived()
    {
        _waitingEventReceived = true;
    }

    private void InvokeEvents(GameEvent[] events)
    {
        if (events == null)
        {
            return;
        }

        foreach (GameEvent gameEvent in events)
        {
            gameEvent?.Invoke();
        }
    }

    private void Log(string message)
    {
        if (_logProgress)
        {
            Debug.LogWarning($"<color=cyan>[StoryManager]</color> {message}");
        }
    }
}
