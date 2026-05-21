using UnityEngine;

public class AttackState : EnemyBaseState
{
    public override void EnterState(EnemyController enemy)
    {
        //Debug.Log("发现敌人！！！！");

        enemy.animState = 2;

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
            enemy.animState = 2;
            enemy.MoveToTarget();
        }
        else
        {
            enemy.animState = 3;
            enemy.AttackAction();
        }
    }


}
