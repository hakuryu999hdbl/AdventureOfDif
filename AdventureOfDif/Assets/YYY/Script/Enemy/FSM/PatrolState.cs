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
        enemy.animState = 0;

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
            enemy.TransitionToState(enemy.attackState);
            return;
        }


        timer -= Time.deltaTime;

        if (isWalking)
        {
            enemy.animState = 1;


            enemy.MovePatrol();

            if (timer <= 0)
            {
                StartIdle(enemy);
                return;
            }
        }
        else
        {
            enemy.animState = 0;

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
        enemy.animState = 0;
        enemy.StopMove();//巡逻停
        timer = Random.Range(enemy.minIdleTime, enemy.maxIdleTime);
    }

    private void StartWalk(EnemyController enemy)
    {
        isWalking = true;
        enemy.animState = 1;
        timer = Random.Range(enemy.minWalkTime, enemy.maxWalkTime);

    }
}
