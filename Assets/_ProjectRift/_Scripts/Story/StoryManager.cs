using UnityEngine;
using System.Collections;
using UnityEngine.Playables;

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
    [SerializeField, ReadOnly] private StoryChapterData _currentChapter;
    [SerializeField, ReadOnly] private int _currentChapterIndex = -1;
    [SerializeField, ReadOnly] private int _currentStepIndex = -1;
    [SerializeField, ReadOnly] private bool _waitingEventReceived;
    [SerializeField, ReadOnly] private GameEvent _activeWaitEvent;
    [SerializeField, ReadOnly] private bool _isPlaying;

    private Coroutine _storyCoroutine;

    public StoryManagerMode PlayMode => _playMode;
    public StoryCampaignData CurrentCampaign => _campaign;
    public StoryChapterData CurrentChapter => _currentChapter;
    public int CurrentChapterIndex => _currentChapterIndex;
    public int CurrentStepIndex => _currentStepIndex;
    public bool IsPlaying => _isPlaying;


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
        _isPlaying = true;
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
        _currentChapterIndex = -1;
        _isPlaying = true;
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
        _currentChapterIndex = -1;
        _currentStepIndex = -1;
        _waitingEventReceived = false;
        _isPlaying = false;
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

            _currentChapterIndex = i;
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
            _currentStepIndex = i;
            yield return ProcessStep(chapter.Steps[i]);
        }

        Log($"Chapter finished: {chapter.ChapterName}");
        _currentStepIndex = -1;
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
        _currentChapterIndex = -1;
        _currentStepIndex = -1;
        _storyCoroutine = null;
        _isPlaying = false;
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
