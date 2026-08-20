using System.Collections;
using UnityEngine;

public class SubmarineProximityDisplay : MonoBehaviour
{
    [SerializeField] private SubmarineProximityRaycaster _raycaster;
    [SerializeField] private bool _activeOnStart = true;
    [SerializeField] private ProximityAxisController[] _axisDisplays = new ProximityAxisController[6];

    [Tooltip("A partir de que punto se activa el parpadeo máximo (0 = Cerca, 1 = Lejos)")]
    [SerializeField, Range(0f, 1f)] private float _maxProximityValue = 0.25f;
    [SerializeField, Range(0f, 1f)] private float _activationDeadZone = 0.02f;

    [Header("Blink")]
    [SerializeField] private float[] _blinkRates = { 1.5f, 3f, 5f, 7f };

    private Coroutine _lightsCoroutine;
    public bool IsOn => _lightsCoroutine != null;

    private void Start()
    {
        if (_activeOnStart) EnableLights();
    }

    [EditorButton("Lights On")]
    public void EnableLights()
    {
        if (_lightsCoroutine != null) return;

        _lightsCoroutine = StartCoroutine(LightsCoroutine());
    }

    [EditorButton("Lights Off")]
    public void DisableLights()
    {
        if (_lightsCoroutine == null) return;

        ClearDisplays();

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
            return;
        }

        int activeSegments = Mathf.CeilToInt(danger * axisDisplay.SegmentCount);
        float blinkRate = GetBlinkRate(activeSegments);
        bool blinkOn = GetBlinkState(blinkRate);

        axisDisplay.SetLights(activeSegments, blinkOn);
    }

    private float RemapDisplayDistance(float distance)
    {
        if (_maxProximityValue <= 0f) return distance;

        return Mathf.InverseLerp(_maxProximityValue, 1f, distance);
    }

    private float GetBlinkRate(int activeSegments)
    {
        if (_blinkRates == null || _blinkRates.Length == 0) return 0f;

        int index = Mathf.Clamp(activeSegments - 1, 0, _blinkRates.Length - 1);
        return Mathf.Max(0f, _blinkRates[index]);
    }

    private bool GetBlinkState(float blinksPerSecond)
    {
        if (blinksPerSecond <= 0f) return true;

        return Mathf.FloorToInt(Time.time * blinksPerSecond * 2f) % 2 == 0;
    }

    private void ClearDisplays()
    {
        for (int i = 0; i < _axisDisplays.Length; i++)
        {
            _axisDisplays[i]?.TurnOff();
        }
    }

    #endregion
}
