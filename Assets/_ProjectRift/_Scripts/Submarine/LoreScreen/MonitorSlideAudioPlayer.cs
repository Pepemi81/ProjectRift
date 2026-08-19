using UnityEngine;

[System.Serializable]
public class MonitorSlideAudioSettings
{
    [Header("Slide Start")]
    [SerializeField] private AudioClip _slideStartClip;
    [SerializeField, Range(0f, 1f)] private float _slideStartVolume = 1f;

    [Header("Typing")]
    [SerializeField] private AudioClip _typingClip;
    [SerializeField] private bool _playTypingPerCharacter = true;
    [SerializeField, Min(1)] private int _typingCharacterInterval = 3;
    [SerializeField, Min(0f)] private float _typingMinDelay = 0.04f;
    [SerializeField, Range(0f, 1f)] private float _typingVolume = 0.5f;
    [SerializeField] private Vector2 _typingPitchRange = new Vector2(0.96f, 1.04f);

    public AudioClip SlideStartClip => _slideStartClip;
    public float SlideStartVolume => _slideStartVolume;
    public AudioClip TypingClip => _typingClip;
    public bool PlayTypingPerCharacter => _playTypingPerCharacter;
    public int TypingCharacterInterval => Mathf.Max(1, _typingCharacterInterval);
    public float TypingMinDelay => Mathf.Max(0f, _typingMinDelay);
    public float TypingVolume => _typingVolume;
    public Vector2 TypingPitchRange => _typingPitchRange;
}

[RequireComponent(typeof(AudioSource))]
public class MonitorSlideAudioPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private MonitorSlideAudioSettings _fallbackSettings;

    private int _charactersSinceTypingSound;
    private float _lastTypingSoundTime = float.NegativeInfinity;

    public MonitorSlideAudioSettings FallbackSettings => _fallbackSettings;

    private void Awake()
    {
        if (_audioSource == null)
        {
            _audioSource = GetComponent<AudioSource>();
        }
    }

    private void Reset()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.playOnAwake = false;
        _audioSource.spatialBlend = 1f;
        _audioSource.rolloffMode = AudioRolloffMode.Linear;
        _audioSource.minDistance = 0.2f;
        _audioSource.maxDistance = 3f;
    }

    private void OnValidate()
    {
        if (_audioSource == null)
        {
            _audioSource = GetComponent<AudioSource>();
        }
    }

    public void BeginSlide(MonitorSlideAudioSettings settings)
    {
        ResetTypingState();
        PlaySlideStart(settings);
    }

    public void PlaySlideStart(MonitorSlideAudioSettings settings = null)
    {
        MonitorSlideAudioSettings resolvedSettings = ResolveSettings(settings);
        if (resolvedSettings == null || resolvedSettings.SlideStartClip == null)
        {
            return;
        }

        PlayOneShot(resolvedSettings.SlideStartClip, resolvedSettings.SlideStartVolume, 1f);
    }

    public void NotifyCharacterRevealed(MonitorSlideAudioSettings settings = null)
    {
        MonitorSlideAudioSettings resolvedSettings = ResolveSettings(settings);
        if (resolvedSettings == null ||
            !resolvedSettings.PlayTypingPerCharacter ||
            resolvedSettings.TypingClip == null)
        {
            return;
        }

        _charactersSinceTypingSound++;
        if (_charactersSinceTypingSound < resolvedSettings.TypingCharacterInterval)
        {
            return;
        }

        if (Time.time - _lastTypingSoundTime < resolvedSettings.TypingMinDelay)
        {
            return;
        }

        _charactersSinceTypingSound = 0;
        _lastTypingSoundTime = Time.time;

        Vector2 pitchRange = resolvedSettings.TypingPitchRange;
        float minPitch = Mathf.Min(pitchRange.x, pitchRange.y);
        float maxPitch = Mathf.Max(pitchRange.x, pitchRange.y);
        float pitch = Random.Range(minPitch, maxPitch);

        PlayOneShot(resolvedSettings.TypingClip, resolvedSettings.TypingVolume, pitch);
    }

    public void ResetTypingState()
    {
        _charactersSinceTypingSound = 0;
        _lastTypingSoundTime = float.NegativeInfinity;
    }

    public void StopSlideAudio()
    {
        if (_audioSource != null)
        {
            _audioSource.Stop();
        }

        ResetTypingState();
    }

    private MonitorSlideAudioSettings ResolveSettings(MonitorSlideAudioSettings settings)
    {
        return settings ?? _fallbackSettings;
    }

    private void PlayOneShot(AudioClip clip, float volume, float pitch)
    {
        if (_audioSource == null || clip == null)
        {
            return;
        }

        _audioSource.pitch = pitch;
        _audioSource.PlayOneShot(clip, volume);
    }
}
