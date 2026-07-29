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

    public override void EnterState(EnemyController enemy)
    {
        enemy.targetPoint = enemy.GetNearestTarget(enemy.attackList);//追寻攻击状态下的目标

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
            CancelSkill(enemy);
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

        enemy.transform.position =
            enemy5.jumpStrikeTargetPos;



        enemy.anim.SetInteger("skillState", 3);
    }

    public void StartAim(EnemyController enemy)
    {
        if (enemy5 == null)
            enemy5 = enemy as Enemy_5;

        if (enemy5 == null)
            return;

        enemy5.isJumpStrikeUntargetable = true;//进入无敌状态

        if (enemy.targetPoint != null)
        {

            enemy5.jumpAimTarget.position =
                enemy5.jumpStrikeTargetPos;
        }

        phase = Phase.Aim;
        timer = enemy5.jumpAimTime;

        enemy5.jumpAimTarget.gameObject.SetActive(true);

        enemy.anim.SetInteger("skillState", 2);

        enemy.shadow.gameObject.SetActive(false);//先把影子藏一下
    }

    public void StartAttack(EnemyController enemy)
    {
        phase = Phase.Attack;

        enemy.StopMove();

        enemy.anim.SetInteger("skillState", 4);
    }

    public void EndSkill(EnemyController enemy)
    {
        if (enemy5 == null)
            enemy5 = enemy as Enemy_5;

        if (enemy5 == null)
            return;

        enemy5.isJumpStrikeUntargetable = false;//离开无敌状态

        enemy.shadow.gameObject.SetActive(true);//显示影子


        if (enemy5.jumpAimTarget != null)
            enemy5.jumpAimTarget.gameObject.SetActive(false);

        if (enemy5.jumpStrikeCollider != null)
            enemy5.jumpStrikeCollider.SetActive(false);



        enemy.anim.SetInteger("skillState", 0);

        if (enemy.isDead)
            return;

        if (enemy.attackList.Count > 0)
            enemy5.TryEnterJumpStrikeOrAttack();//随机是否进入再度技能状态
        else
            enemy.TransitionToState(enemy.patrolState);
    }

    private void CancelSkill(EnemyController enemy)
    {
        if (enemy5 != null)
        {
            if (enemy5.jumpAimTarget != null)
                enemy5.jumpAimTarget.gameObject.SetActive(false);

            if (enemy5.jumpStrikeCollider != null)
                enemy5.jumpStrikeCollider.SetActive(false);


        }

        enemy5.isJumpStrikeUntargetable = false;//离开无敌状态

        enemy.shadow.gameObject.SetActive(true);//显示影子

        enemy.anim.SetInteger("skillState", 0);
        enemy.TransitionToState(enemy.hitState);
    }

    public override void ExitState(EnemyController enemy)
    {
        enemy.StopMove();

        if (enemy5 != null &&
            enemy5.jumpAimTarget != null)
        {
            enemy5.jumpAimTarget.gameObject.SetActive(false);
        }
    }
}