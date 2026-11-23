using UnityEngine;

public class JumpStrikeSkillState : EnemyBaseState
{
    private enum Phase
    {
        Jump,
        Aim,
        Fall,
        Attack
    }

    private Phase phase;
    private float timer;

    private Enemy_5 enemy5;

    public bool CanHit => enemy5 != null && !enemy5.isDead &&
        (phase == Phase.Fall || phase == Phase.Attack);

    public override void EnterState(EnemyController enemy)
    {
        enemy5 = enemy as Enemy_5;

        if (enemy5 == null)
        {
            enemy.TransitionToState(enemy.patrolState);
            return;
        }

        enemy.SetStateColor(enemy.JumpStrikeColor);

        enemy.StopMove();

        enemy.SetAnimState(4);

        phase = Phase.Jump;

        enemy5.jumpStrikeTargetPos = enemy.targetPoint != null
            ? (Vector2)enemy.targetPoint.position + enemy5.landingOffset
            : (Vector2)enemy.transform.position;

        enemy.anim.SetInteger("skillState", 1);

        if (enemy5.jumpAimTarget != null)
            enemy5.jumpAimTarget.gameObject.SetActive(false);

        if (enemy5.jumpStrikeCollider != null)
            enemy5.jumpStrikeCollider.SetActive(false);
    }

    public override void OnUpdate(EnemyController enemy)
    {
        if (enemy.isDead)
            return;

        if (enemy.isHurt)
        {
            enemy.TransitionToState(enemy.hitState);
            return;
        }

        switch (phase)
        {
            case Phase.Jump:
                enemy.StopMove();
                break;

            case Phase.Aim:
                UpdateAim(enemy);
                break;

            case Phase.Fall:
                enemy.StopMove();
                break;

            case Phase.Attack:
                enemy.StopMove();
                break;
        }
    }

    private void UpdateAim(EnemyController enemy)
    {
        enemy.StopMove();

        timer -= Time.deltaTime;

        if (enemy.targetPoint != null)
        {
            enemy5.jumpStrikeTargetPos =
                (Vector2)enemy.targetPoint.position +
                enemy5.landingOffset;

            if (enemy5.jumpAimTarget != null)
            {
                enemy5.jumpAimTarget.position =
                    enemy5.jumpStrikeTargetPos;
            }
        }

        if (timer <= 0f)
        {
            LockTargetAndFall(enemy);
        }
    }

    private void LockTargetAndFall(EnemyController enemy)
    {
        phase = Phase.Fall;

      

        if (enemy5.jumpAimTarget != null)
            enemy5.jumpAimTarget.gameObject.SetActive(false);

        Vector3 landingPosition = enemy5.jumpStrikeTargetPos;
        landingPosition.z = enemy.transform.position.z;
        if (enemy.aiPath != null)
            enemy.aiPath.Teleport(landingPosition);
        else
            enemy.transform.position = landingPosition;



        enemy.anim.SetInteger("skillState", 3);
    }

    public void StartAim(EnemyController enemy)
    {
        if (enemy5 == null || enemy.isDead || phase != Phase.Jump)
            return;

        enemy5.isJumpStrikeUntargetable = true;//进入无敌状态

        if (enemy.targetPoint != null)
            enemy5.jumpStrikeTargetPos = (Vector2)enemy.targetPoint.position + enemy5.landingOffset;

        phase = Phase.Aim;
        timer = enemy5.jumpAimTime;

        if (enemy5.jumpAimTarget != null)
        {
            enemy5.jumpAimTarget.position = enemy5.jumpStrikeTargetPos;
            enemy5.jumpAimTarget.gameObject.SetActive(true);
        }

        enemy.anim.SetInteger("skillState", 2);

        if (enemy.shadow != null)
            enemy.shadow.gameObject.SetActive(false);//先把影子藏一下
    }

    public void StartAttack(EnemyController enemy)
    {
        if (enemy5 == null || enemy.isDead || phase != Phase.Fall)
            return;

        phase = Phase.Attack;

        enemy.StopMove();

        enemy.anim.SetInteger("skillState", 4);
    }

    public void EndSkill(EnemyController enemy)
    {
        if (enemy5 != null && !enemy.isDead && phase == Phase.Attack)
            enemy5.RecoverFromJumpStrike();
    }

    public override void ExitState(EnemyController enemy)
    {
        if (enemy5 == null)
            return;

        enemy.StopMove();
        enemy5.isJumpStrikeUntargetable = false;
        if (enemy.shadow != null)
            enemy.shadow.gameObject.SetActive(true);
        if (enemy5.jumpAimTarget != null)
            enemy5.jumpAimTarget.gameObject.SetActive(false);
        if (enemy5.jumpStrikeCollider != null)
            enemy5.jumpStrikeCollider.SetActive(false);

        enemy.anim.SetInteger("skillState", 0);
        enemy5 = null;
    }
}
