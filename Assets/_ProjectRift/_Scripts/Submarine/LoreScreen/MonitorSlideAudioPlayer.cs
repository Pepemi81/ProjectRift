using UnityEngine;

[System.Serializable]
public class MonitorSlideAudioSettings
{
    [Header("On Slide Start")]
    [SerializeField] private AudioClip _slideStartSound;
    [SerializeField, Range(0f, 1f)] private float _slideStartVolume = 1f;

    [Header("On Typing")]
    [SerializeField] private AudioClip _typingSound;
    [Tooltip("Cada cuantos caracteres se permite sonar")]
    [SerializeField, Min(1)] private int _characterInterval = 2;
    [SerializeField, Range(0f, 1f)] private float _typingVolume = 0.5f;
    [SerializeField] private Vector2 _typingPitchRange = new Vector2(0.96f, 1.04f);

    public AudioClip SlideStartSound => _slideStartSound;
    public float SlideStartVolume => _slideStartVolume;
    public AudioClip TypingSound => _typingSound;
    public int TypingCharacterInterval => Mathf.Max(1, _characterInterval);
    public float TypingVolume => _typingVolume;
    public Vector2 TypingPitchRange => _typingPitchRange;
}

[RequireComponent(typeof(AudioSource))]
public class MonitorSlideAudioPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private MonitorSlideAudioSettings _fallbackSettings;

    private int _charactersSinceTypingSound;
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
        _charactersSinceTypingSound = 0;
        PlaySlideStartSound(settings);
    }

    public void PlaySlideStartSound(MonitorSlideAudioSettings settings = null)
    {
        if (settings.SlideStartSound == null) return;

        PlayOneShot(settings.SlideStartSound, settings.SlideStartVolume, 1f);
    }

    public void NotifyCharacterRevealed(MonitorSlideAudioSettings settings)
    {
        if (settings.TypingSound == null) return;

        _charactersSinceTypingSound++;

        if (_charactersSinceTypingSound < settings.TypingCharacterInterval) return;


        _charactersSinceTypingSound = 0;

        Vector2 pitchRange = settings.TypingPitchRange;
        float minPitch = Mathf.Min(pitchRange.x, pitchRange.y);
        float maxPitch = Mathf.Max(pitchRange.x, pitchRange.y);
        float pitch = Random.Range(minPitch, maxPitch);

        PlayOneShot(settings.TypingSound, settings.TypingVolume, pitch);
    }

    public void StopSlideAudio()
    {
        _audioSource.Stop();
        _charactersSinceTypingSound = 0;
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
