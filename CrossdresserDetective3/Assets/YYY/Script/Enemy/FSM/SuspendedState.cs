using UnityEngine;

public class SuspendedState : EnemyBaseState
{
    public override void EnterState(EnemyController enemy) { enemy.EnterSuspendedPose(); }
    public override void OnUpdate(EnemyController enemy) { enemy.HoldSuspendedPosition(); }
    public override void ExitState(EnemyController enemy) { }
}
