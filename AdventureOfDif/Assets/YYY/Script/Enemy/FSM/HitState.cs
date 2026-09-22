using UnityEngine;

public class HitState : EnemyBaseState
{
    public override void EnterState(EnemyController enemy)
    {
        enemy.SetStateColor(enemy.hitColor);


        //清理状态
        enemy.animState = 0;
        enemy.anim.SetInteger("state", 0);
        enemy.anim.ResetTrigger("attack");



        enemy.StopMove();
    }

    public override void OnUpdate(EnemyController enemy)
    {
        Debug.Log(
       $"HitState运行中：{enemy.name}  " +
       $"hurtPhase={enemy.hurtPhase} " +
       $"isHurt={enemy.isHurt} " +
       $"isCatching={enemy.isCatching}"
   );


        enemy.StopMove(); // 防止AIPath被别的地方重新打开

        if (enemy.UpdateHurtMotion())
        {
            enemy.OnDamageOver();
        }
    }
}