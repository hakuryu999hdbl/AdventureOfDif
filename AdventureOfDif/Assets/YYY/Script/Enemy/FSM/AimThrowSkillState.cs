using UnityEngine;

public class AimThrowSkillState : EnemyBaseState
{
    private enum Phase
    {
        Ready,
        Throw,
        Laugh
    }

    private Phase phase;
    private float timer;
    private bool hasSpawned;

    public override void EnterState(EnemyController enemy)
    {
        enemy.SetStateColor(enemy.AimThrowSkillColor);


        phase = Phase.Ready;
        timer = enemy.throwAimTime;
        hasSpawned = false;

        enemy.StopMove();
        enemy.aiPath.canMove = false;

        enemy.anim.SetInteger("skillState", 1);
        enemy.SetAnimState(4);

        if (enemy.throwAimTarget != null)
            enemy.throwAimTarget.gameObject.SetActive(true);
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

            case Phase.Throw:
                break;

            case Phase.Laugh:
                break;
        }
    }

    private void UpdateReady(EnemyController enemy)
    {
        timer -= Time.deltaTime;

        enemy.StopMove();

        if (enemy.targetPoint != null)
        {
            enemy.throwTargetPos = enemy.targetPoint.position;

            if (enemy.throwAimTarget != null)
                enemy.throwAimTarget.position = enemy.throwTargetPos;

            enemy.FaceToPosition(enemy.throwTargetPos);
        }

        if (timer <= 0f)
        {
            phase = Phase.Throw;
            enemy.anim.SetInteger("skillState", 2);

            enemy.throwAimTarget.gameObject.SetActive(false);//瞄准消失
        }
    }

    public void SpawnThrowExplosion(EnemyController enemy)
    {
        if (hasSpawned) return;
        hasSpawned = true;

        if (enemy.throwExplosionPrefab != null)
        {
            Object.Instantiate(
                enemy.throwExplosionPrefab,
                enemy.throwTargetPos,
                Quaternion.identity
            );
        }
    }

    public void StartLaugh(EnemyController enemy)
    {
        phase = Phase.Laugh;
        enemy.anim.SetInteger("skillState", 3);
    }

    public override void ExitState(EnemyController enemy)
    {
        enemy.StopMove();
        enemy.anim.SetInteger("skillState", 0);
        enemy.SetAnimState(0);

        if (enemy.throwAimTarget != null)
            enemy.throwAimTarget.gameObject.SetActive(false);
    }
}