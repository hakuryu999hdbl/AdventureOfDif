using UnityEngine;

public class BlockState : EnemyBaseState
{
    public override void EnterState(EnemyController enemy)
    {
        Debug.Log("正式进入 BlockState");
        enemy.SetStateColor(enemy.blockColor);

        enemy.StopMove();
        enemy.SetAnimState(0);

        enemy.anim.ResetTrigger("attack");
        enemy.anim.SetInteger("skillState", 1);
        enemy.SetAnimState(4);
    }

    public override void OnUpdate(EnemyController enemy)
    {
        enemy.StopMove();
    }

    public override void ExitState(EnemyController enemy)
    {
    }
}