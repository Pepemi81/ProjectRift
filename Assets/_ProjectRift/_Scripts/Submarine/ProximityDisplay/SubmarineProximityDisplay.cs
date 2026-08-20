using System.Collections;
using UnityEngine;

[System.Serializable]
public class ProximityBlinkPattern
{
    [SerializeField, Min(0f)] private float _onDuration = 1f;
    [SerializeField, Min(0f)] private float _offDuration = 0.125f;

    public float OnDuration => _onDuration;
    public float OffDuration => _offDuration;

    public ProximityBlinkPattern(float onDuration, float offDuration)
    {
        _onDuration = onDuration;
        _offDuration = offDuration;
    }
}
public class SubmarineProximityDisplay : MonoBehaviour
{

    [SerializeField] private SubmarineProximityRaycaster _raycaster;
    [SerializeField] private bool _activeOnStart = true;
    [SerializeField] private ProximityAxisController[] _axisDisplays = new ProximityAxisController[6];

    [Tooltip("A partir de que punto se activa el parpadeo máximo (0 = Cerca, 1 = Lejos)")]
    [SerializeField, Range(0f, 1f)] private float _maxProximityValue = 0.25f;
    [SerializeField, Range(0f, 1f)] private float _activationDeadZone = 0.02f;

    [Header("Blink")]
    [SerializeField] private bool _synchronizeBlink = true;
    [SerializeField] private ProximityBlinkPattern[] _blinkPatterns =
    {
        new ProximityBlinkPattern(1f, 0.125f),
        new ProximityBlinkPattern(0.6f, 0.125f),
        new ProximityBlinkPattern(0.3f, 0.1f),
        new ProximityBlinkPattern(0.12f, 0.08f)
    };

    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _blinkClip;
    [SerializeField, Range(0f, 1f)] private float _blinkVolume = 0.5f;
    [SerializeField] private Vector2 _blinkPitchRange = new Vector2(1f, 1f);

    private Coroutine _lightsCoroutine;
    private readonly bool[] _previousBlinkStates = new bool[6];
    private readonly bool[] _axisActiveStates = new bool[6];
    private readonly float[] _axisBlinkStartTimes = new float[6];
    public bool IsOn => _lightsCoroutine != null;

    private void Awake()
    {
        if (_audioSource == null)
        {
            _audioSource = GetComponent<AudioSource>();
        }
    }

    private void Start()
    {
        if (_activeOnStart) EnableLights();
    }

    [EditorButton("Lights On")]
    public void EnableLights()
    {
        if (_lightsCoroutine != null) return;

        ResetBlinkStates();
        _lightsCoroutine = StartCoroutine(LightsCoroutine());
    }

    [EditorButton("Lights Off")]
    public void DisableLights()
    {
        if (_lightsCoroutine == null) return;

        ClearDisplays();
        ResetBlinkStates();

        StopCoroutine(_lightsCoroutine);
        _lightsCoroutine = null;
    }

    #region Functionality

    private IEnumerator LightsCoroutine()
    {
        while (true)
        {
            if (_raycaster == null)
            {
                ClearDisplays();
                yield return new WaitForEndOfFrame();
                continue;
            }

            for (int i = 0; i < _axisDisplays.Length; i++)
            {
                ProximityAxisController axisDisplay = _axisDisplays[i];

                if (axisDisplay == null || axisDisplay.SegmentCount == 0) continue;
                SetAxisLights(axisDisplay);
            }

            yield return new WaitForEndOfFrame();
        }
    }

    private void SetAxisLights(ProximityAxisController axisDisplay)
    {
        float distance = _raycaster.GetValue(axisDisplay.Axis);
        float displayDistance = RemapDisplayDistance(distance);
        float danger = Mathf.Clamp01(1f - displayDistance);

        if (danger <= _activationDeadZone)
        {
            axisDisplay.TurnOff();
            SetAxisActiveState(axisDisplay.Axis, false);
            SetPreviousBlinkState(axisDisplay.Axis, false);
            return;
        }

        int activeSegments = Mathf.CeilToInt(danger * axisDisplay.SegmentCount);
        EnsureAxisActive(axisDisplay.Axis);
        bool blinkOn = GetBlinkState(axisDisplay.Axis, activeSegments);

        axisDisplay.SetLights(activeSegments, blinkOn);
        TryPlayBlinkSound(axisDisplay.Axis, blinkOn);
    }

    private float RemapDisplayDistance(float distance)
    {
        if (_maxProximityValue <= 0f) return distance;

        return Mathf.InverseLerp(_maxProximityValue, 1f, distance);
    }

    private bool GetBlinkState(ProximityAxis axis, int activeSegments)
    {
        ProximityBlinkPattern pattern = GetBlinkPattern(activeSegments);
        if (pattern == null) return true;

        float cycleDuration = pattern.OnDuration + pattern.OffDuration;
        if (cycleDuration <= 0f) return true;

        float time = _synchronizeBlink
            ? Time.time
            : Time.time - _axisBlinkStartTimes[(int)axis];

        float cycleTime = time % cycleDuration;
        return cycleTime < pattern.OnDuration;
    }

    private ProximityBlinkPattern GetBlinkPattern(int activeSegments)
    {
        if (_blinkPatterns == null || _blinkPatterns.Length == 0) return null;

        int index = Mathf.Clamp(activeSegments - 1, 0, _blinkPatterns.Length - 1);
        return _blinkPatterns[index];
    }

    private void ClearDisplays()
    {
        for (int i = 0; i < _axisDisplays.Length; i++)
        {
            _axisDisplays[i]?.TurnOff();
        }
    }

    private void TryPlayBlinkSound(ProximityAxis axis, bool blinkOn)
    {
        int index = (int)axis;
        bool previousBlinkOn = _previousBlinkStates[index];
        _previousBlinkStates[index] = blinkOn;

        if (!blinkOn || previousBlinkOn || _audioSource == null || _blinkClip == null)
        {
            return;
        }

        float minPitch = Mathf.Min(_blinkPitchRange.x, _blinkPitchRange.y);
        float maxPitch = Mathf.Max(_blinkPitchRange.x, _blinkPitchRange.y);
        _audioSource.pitch = Random.Range(minPitch, maxPitch);
        _audioSource.PlayOneShot(_blinkClip, _blinkVolume);
    }


    private void EnsureAxisActive(ProximityAxis axis)
    {
        int index = (int)axis;
        if (_axisActiveStates[index]) return;

        _axisActiveStates[index] = true;
        _axisBlinkStartTimes[index] = Time.time;
        _previousBlinkStates[index] = false;
    }

    private void SetAxisActiveState(ProximityAxis axis, bool isActive)
    {
        _axisActiveStates[(int)axis] = isActive;
    }
    private void SetPreviousBlinkState(ProximityAxis axis, bool blinkOn)
    {
        _previousBlinkStates[(int)axis] = blinkOn;
    }

    private void ResetBlinkStates()
    {
        for (int i = 0; i < _previousBlinkStates.Length; i++)
        {
            _previousBlinkStates[i] = false;
            _axisActiveStates[i] = false;
            _axisBlinkStartTimes[i] = 0f;
        }
    }
    #endregion
}
