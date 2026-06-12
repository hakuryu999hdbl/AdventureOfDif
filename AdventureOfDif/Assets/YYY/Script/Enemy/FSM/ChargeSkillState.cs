using UnityEngine;

public class ChargeSkillState : EnemyBaseState
{
    private enum Phase
    {
        Ready,
        Move,
        Hit
    }

    private Phase phase;
    private float timer;

    public override void EnterState(EnemyController enemy)
    {
        enemy.SetStateColor(enemy.ChargeSkillColor);

        phase = Phase.Ready;
        timer = enemy.chargeReadyTime;

        enemy.StopMove();

        enemy.aiPath.canMove = false;
        enemy.anim.SetInteger("skillState", 1);
        enemy.SetAnimState(4);

        enemy.chargeTargetPos = enemy.transform.position;

      
    }

    public override void OnUpdate(EnemyController enemy)
    {
        if (enemy.isDead || enemy.isHurt) return;

        if (enemy.attackList == null || enemy.attackList.Count <= 0)
        {
            enemy.TransitionToState(enemy.patrolState);
            return;
        }

        enemy.targetPoint = enemy.attackList[0];




        switch (phase)
        {
            case Phase.Ready:
                UpdateReady(enemy);
                break;

            case Phase.Move:
                UpdateMove(enemy);
                break;

            case Phase.Hit:
                break;
        }

        if (enemy.aiPath.desiredVelocity.x > 0.05f)
            enemy.transform.localScale = new Vector3(1, 1, 1);
        else if (enemy.aiPath.desiredVelocity.x < -0.05f)
            enemy.transform.localScale = new Vector3(-1, 1, 1);
    }

    private void UpdateReady(EnemyController enemy)
    {
        enemy.StopMove();

        timer -= Time.deltaTime;

        if (enemy.targetPoint != null)
        {
            enemy.chargeTargetPos = enemy.targetPoint.position;

            // TODO：瞄准UI跟随玩家
            // enemy.aimUI.SetPosition(enemy.chargeTargetPos);
        }

        if (timer <= 0f)
        {
            // TODO：瞄准UI闪一下
            // enemy.aimUI.LockFlash();
            enemy.aimUI.SetTrigger("Flash");

            phase = Phase.Move;
            enemy.anim.SetInteger("skillState", 2);

            enemy.enemyTarget.position = enemy.chargeTargetPos;
            enemy.aiPath.canMove = true;
            enemy.aiPath.maxSpeed = enemy.chargeSpeed;
        }
    }

    private void UpdateMove(EnemyController enemy)
    {
        enemy.aiPath.maxSpeed = enemy.chargeSpeed;

        float distance = Vector2.Distance(enemy.transform.position, enemy.chargeTargetPos);

        if (distance <= enemy.chargeStopDistance)
        {
            enemy.StopMove();

            phase = Phase.Hit;
            enemy.anim.SetInteger("skillState", 3);
        }

        // TODO：显示瞄准UI
        enemy.aimUI.gameObject.SetActive(true);
    }

    public override void ExitState(EnemyController enemy)
    {
        enemy.StopMove();
        enemy.anim.SetInteger("skillState", 0);
        enemy.SetAnimState(0);

        // TODO：隐藏瞄准UI
        enemy.aimUI.gameObject.SetActive(false);
    }


}