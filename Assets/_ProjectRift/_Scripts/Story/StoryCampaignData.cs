using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Story/Campaign")]
public class StoryCampaignData : ScriptableObject
{
    [SerializeField] private string _campaignName;
    [SerializeField, TextArea(3, 8)] private string _summary;
    [SerializeField] private List<StoryChapterData> _chapters = new List<StoryChapterData>();

    public string CampaignName => _campaignName;
    public string Summary => _summary;
    public List<StoryChapterData> Chapters => _chapters;
}
