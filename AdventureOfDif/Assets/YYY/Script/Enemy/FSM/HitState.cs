using UnityEngine;

public class HitState : EnemyBaseState
{
    public override void EnterState(EnemyController enemy)
    {
        enemy.StopMove();
      
    }

    public override void OnUpdate(EnemyController enemy)
    {
        enemy.StopMove();// 防止AIPath被别的地方重新打开
    }
}