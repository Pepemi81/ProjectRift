using DG.Tweening;
using UnityEngine;

public class MissionTest : MissionAreaBase
{
    protected override void OnSubmarineEntered(Collider submarine)
    {
        base.OnSubmarineEntered(submarine);
        
        DebugText.Instance.SetText("Mission Started: Test Mission", 3f);

        DOVirtual.DelayedCall(10f, FinishMission);
    }
}
