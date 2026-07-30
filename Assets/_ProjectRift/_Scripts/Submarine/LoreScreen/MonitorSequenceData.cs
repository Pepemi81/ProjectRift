using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject que almacena una lista ordenada de diapositivas (MonitorSlideData) 
/// para reproducirlas de forma consecutiva en el monitor.
/// </summary>
[CreateAssetMenu(menuName = "ScriptableObjects/Monitor/Slide Sequence")]
public class MonitorSequenceData : ScriptableObject
{
    [SerializeField] private List<MonitorSlideData> _slides = new List<MonitorSlideData>();
    [SerializeField] private GameEvent _eventOnComplete;

    public List<MonitorSlideData> Slides => _slides;
    public GameEvent EventOnComplete => _eventOnComplete;
}
