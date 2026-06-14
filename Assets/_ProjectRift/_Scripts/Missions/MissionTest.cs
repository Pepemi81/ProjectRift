using UnityEngine;
using DG.Tweening;

public class MissionTest : MissionAreaBase
{
    protected override void OnSubmarineEntered(Collider submarine)
    {
        base.OnSubmarineEntered(submarine);
        
        DebugText.Instance.SetText("Mission Started: Test Mission", 3f);

        DOVirtual.DelayedCall(10f, FinishMission);
    }

    [EditorButton("Test")]
    private void Test()
    {
        DebugText.Instance.SetText("Test print", 3f);
    }
}
