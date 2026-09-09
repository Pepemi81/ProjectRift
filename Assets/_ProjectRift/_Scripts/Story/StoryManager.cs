using UnityEngine;
using System.Collections;
using UnityEngine.Playables;
using UnityEngine.Serialization;

public enum StoryManagerMode
{
    Story,
    DebugChapter
}

public class StoryManager : Singleton<StoryManager>
{
    [Header("Mode")]
    [SerializeField] private StoryManagerMode _playMode = StoryManagerMode.Story;
    [SerializeField] private bool _playOnStart = true;

    [SerializeField] private StoryCampaignData _campaign;
    [SerializeField] private StoryChapterData _debugChapter;

    [Header("Timeline")]
    [SerializeField] private PlayableDirector _timelineDirector;

    [Header("Debug")]
    [SerializeField] private bool _logProgress = true;

    private Coroutine _storyCoroutine;
    private bool _waitingEventReceived;
    private GameEvent _activeWaitEvent;
    private StoryChapterData _currentChapter;

    public StoryManagerMode PlayMode => _playMode;
    public StoryCampaignData CurrentCampaign => _campaign;
    public StoryChapterData CurrentChapter => _currentChapter;
    public int CurrentChapterIndex { get; private set; } = -1;
    public int CurrentStepIndex { get; private set; } = -1;
    public bool IsPlaying => _storyCoroutine != null;


    private void Start()
    {
        if (!_playOnStart)
        {
            return;
        }

        if (_playMode == StoryManagerMode.Story)
        {
            PlayCampaign(_campaign);
        }
        else
        {
            PlayChapter(_debugChapter);
        }
    }

    private void OnDisable()
    {
        StopCurrentStory();
    }


    public void PlayCampaign(StoryCampaignData campaign)
    {
        if (campaign == null)
        {
            Debug.LogWarning("<color=orange>[StoryManager]</color> Cannot play a null campaign.");
            return;
        }

        StopCurrentStory();
        _campaign = campaign;
        _playMode = StoryManagerMode.Story;
        _storyCoroutine = StartCoroutine(ProcessCampaign(campaign));
    }

    public void PlayChapter(StoryChapterData chapter)
    {
        if (chapter == null)
        {
            Debug.LogWarning("<color=orange>[StoryManager]</color> Cannot play a null chapter.");
            return;
        }

        StopCurrentStory();
        _debugChapter = chapter;
        _playMode = StoryManagerMode.DebugChapter;
        CurrentChapterIndex = -1;
        _storyCoroutine = StartCoroutine(ProcessSingleChapter(chapter));
    }

    public void StopCurrentChapter()
    {
        StopCurrentStory();
    }

    public void StopCurrentStory()
    {
        if (_storyCoroutine != null)
        {
            StopCoroutine(_storyCoroutine);
            _storyCoroutine = null;
        }

        UnsubscribeFromActiveWaitEvent();
        _currentChapter = null;
        CurrentChapterIndex = -1;
        CurrentStepIndex = -1;
        _waitingEventReceived = false;
    }


    #region Story Flow

    private IEnumerator ProcessCampaign(StoryCampaignData campaign)
    {
        Log($"Starting campaign: {campaign.CampaignName}");

        for (int i = 0; i < campaign.Chapters.Count; i++)
        {
            StoryChapterData chapter = campaign.Chapters[i];

            if (chapter == null)
            {
                Debug.LogWarning($"<color=orange>[StoryManager]</color> Campaign '{campaign.CampaignName}' has a null chapter at index {i}.");
                continue;
            }

            CurrentChapterIndex = i;
            yield return ProcessChapter(chapter);
        }

        Log($"Campaign finished: {campaign.CampaignName}");
        ClearPlaybackState();
    }

    private IEnumerator ProcessSingleChapter(StoryChapterData chapter)
    {
        yield return ProcessChapter(chapter);

        ClearPlaybackState();
    }

    private IEnumerator ProcessChapter(StoryChapterData chapter)
    {
        _currentChapter = chapter;
        Log($"Starting chapter: {chapter.ChapterName}");

        for (int i = 0; i < chapter.Steps.Count; i++)
        {
            CurrentStepIndex = i;
            yield return ProcessStep(chapter.Steps[i]);
        }

        Log($"Chapter finished: {chapter.ChapterName}");
        CurrentStepIndex = -1;
    }

    #endregion

    #region Step Flow

    private IEnumerator ProcessStep(StoryStepData step)
    {
        if (step == null) yield break;

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
            Debug.LogWarning($"<color=orange>[StoryManager]</color> Timeline director has not been assigned");
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

    #endregion

    #region Events logic

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

    #endregion

    #region Utility

    private void ClearPlaybackState()
    {
        _currentChapter = null;
        CurrentChapterIndex = -1;
        CurrentStepIndex = -1;
        _storyCoroutine = null;
    }

    private void Log(string message)
    {
        if (_logProgress)
        {
            Debug.LogWarning($"<color=cyan>[StoryManager]</color> {message}");
        }
    }

    #endregion
}
