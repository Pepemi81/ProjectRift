using UnityEngine;
using DG.Tweening;

public class MissionTest : MissionAreaBase
{
    protected override void OnSubmarineEntered(Collider submarine)
    {
        base.OnSubmarineEntered(submarine);
        
        //DebugText.Instance.SetText("Mission Started: Test Mission", 3f);
        Debug.LogWarning("<Color=Red>Mission Started: Test Mission</Color>");
    }
}
