using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "ScriptableObjects/Story/Chapter")]
public class StoryChapterData : ScriptableObject
{
    [SerializeField] private string _chapterName;
    [SerializeField, TextArea(3, 8)] private string _summary;
    [SerializeField] private List<StoryStepData> _steps = new List<StoryStepData>();

    public string ChapterName => _chapterName;
    public string Summary => _summary;
    public List<StoryStepData> Steps => _steps;
}
