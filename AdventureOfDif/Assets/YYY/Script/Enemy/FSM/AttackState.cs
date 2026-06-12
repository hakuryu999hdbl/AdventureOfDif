using UnityEngine;

public class AttackState : EnemyBaseState
{
    public override void EnterState(EnemyController enemy)
    {
        enemy.SetStateColor(enemy.attackColor);

        //Debug.Log("发现敌人！！！！");

        enemy.SetAnimState(2); // 进入攻击状态，先恢复追逐动画状态


        if (enemy.attackList == null || enemy.attackList.Count <= 0)
        {
            enemy.targetPoint = null;
            enemy.TransitionToState(enemy.patrolState);
            return;
        }



        enemy.targetPoint = enemy.attackList[0];
    }

    public override void OnUpdate(EnemyController enemy)
    {

        if (enemy.isDead)
        {
            return;
        }
        if (enemy.isHurt)
        {
            enemy.StopMove();
            enemy.TransitionToState(enemy.hitState);//进入受击状态
            return;
        }//特殊增加进入受击状态入口

        enemy.attackList.RemoveAll(t => t == null || !t.CompareTag("Player"));

        if (enemy.attackList.Count <= 0)
        {
            enemy.targetPoint = null;
            enemy.TransitionToState(enemy.patrolState);
            return;
        }

        enemy.targetPoint = enemy.GetNearestTarget(enemy.attackList);

        if (enemy.targetPoint == null)
        {
            enemy.TransitionToState(enemy.patrolState);
            return;
        }

        float distance = Vector2.Distance(enemy.transform.position, enemy.targetPoint.position);

        if (distance > enemy.attackRange)
        {
            enemy.SetAnimState(2);
            enemy.MoveToTarget();
        }
        else
        {
            enemy.SetAnimState(3);
            enemy.AttackAction();
        }

        
    }


}
