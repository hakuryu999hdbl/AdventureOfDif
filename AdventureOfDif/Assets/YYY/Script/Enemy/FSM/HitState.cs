using UnityEngine;

public class HitState : EnemyBaseState
{
    public override void EnterState(EnemyController enemy)
    {
        enemy.SetStateColor(enemy.hitColor);


        enemy.StopMove();//受伤停
      
    }

    public override void OnUpdate(EnemyController enemy)
    {
        enemy.StopMove();// 防止AIPath被别的地方重新打开
    }
}