using UnityEngine;

public class HitState : EnemyBaseState
{
    public override void EnterState(EnemyController enemy)
    {
        enemy.SetStateColor(enemy.hitColor);
        //enemy.animState = 4; // 受击/倒地用。没有4号动画就改回你的受击编号。
        enemy.StopMove();
    }

    public override void OnUpdate(EnemyController enemy)
    {
        enemy.StopMove(); // 防止AIPath被别的地方重新打开

        if (enemy.UpdateHurtMotion())
        {
            enemy.OnDamageOver();
        }
    }
}