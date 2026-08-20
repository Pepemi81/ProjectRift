using UnityEngine;

[System.Serializable]
public class ProximityAxisController
{
    [SerializeField] private ProximityAxis _axis;
    [SerializeField] private bool _fillFromCenter = true;
    [SerializeField] private ProximitySegmentVisual[] _segments;
    [SerializeField] private Light _axisLight;
    [SerializeField, Min(0f)] private float _axisLightIntensity = 1f;

    public ProximityAxis Axis => _axis;
    public int SegmentCount => _segments != null ? _segments.Length : 0;

    public void Apply(int activeSegments, bool blinkOn)
    {
        int segmentCount = SegmentCount;
        activeSegments = Mathf.Clamp(activeSegments, 0, segmentCount);

        bool anySegmentOn = false;

        for (int i = 0; i < segmentCount; i++)
        {
            bool isActive = IsSegmentActive(i, activeSegments, segmentCount);
            bool isOn = isActive && blinkOn;

            if (_segments[i] != null)
            {
                _segments[i].SetState(isOn);
            }

            anySegmentOn |= isOn;
        }

        if (_axisLight != null)
        {
            _axisLight.enabled = anySegmentOn;
            _axisLight.intensity = anySegmentOn ? _axisLightIntensity : 0f;
        }
    }

    public void Clear()
    {
        Apply(0, false);
    }

    private bool IsSegmentActive(int index, int activeSegments, int segmentCount)
    {
        if (activeSegments <= 0)
        {
            return false;
        }

        return _fillFromCenter
            ? index < activeSegments
            : index >= segmentCount - activeSegments;
    }
}
