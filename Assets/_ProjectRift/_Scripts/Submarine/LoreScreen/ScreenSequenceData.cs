using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Monitor/Screen Sequence")]
public class ScreenSequenceData : ScriptableObject
{
    [SerializeField] private List<ScreenDisplayData> _screens = new List<ScreenDisplayData>();
    public List<ScreenDisplayData> Screens => _screens;
}