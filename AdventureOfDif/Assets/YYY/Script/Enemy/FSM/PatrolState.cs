using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : EnemyBaseState
{
    private float timer;
    private bool isWalking;
    public override void EnterState(EnemyController enemy)
    {
        enemy.SetStateColor(enemy.patrolColor);

        //每次进入巡逻状态时重新投放巡逻目标（通过删除目前的）
        enemy.targetPoint = null;
        enemy.ForceResetPatrolTarget();

        enemy.CleanState();

        StartIdle(enemy);

    }

    public override void OnUpdate(EnemyController enemy)
    {

        if (enemy.isDead || enemy.isHurt)
        {
            return;
        }

        if (enemy.attackList.Count > 0)
        {

            //if (enemy is Enemy_1)
            //{
            //    enemy.TransitionToState(enemy.chargeSkillState);
            //}
            //else
            //{
            //    enemy.TransitionToState(enemy.attackState);
            //}

            enemy.EnterBattleState();//虚类进入战斗

            return;
        }


        timer -= Time.deltaTime;

        if (isWalking)
        {


            enemy.MovePatrol();

            if (timer <= 0)
            {
                StartIdle(enemy);
                return;
            }
        }
        else
        {


            if (timer <= 0)
            {
                StartWalk(enemy);
                return;
            }
        }


    }

    private void StartIdle(EnemyController enemy)
    {
        isWalking = false;
        enemy.SetAnimState(0);
        enemy.StopMove();//巡逻停
        timer = Random.Range(enemy.minIdleTime, enemy.maxIdleTime);
    }

    private void StartWalk(EnemyController enemy)
    {
        isWalking = true;
        enemy.SetAnimState(1);
        timer = Random.Range(enemy.minWalkTime, enemy.maxWalkTime);

    }
}
