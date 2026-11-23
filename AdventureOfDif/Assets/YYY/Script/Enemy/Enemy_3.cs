using UnityEngine;

public class Enemy_3 : EnemyController
{
    [Header("横向冲刺抓投")]
    [SerializeField, Min(0f)] private float minDashDistance = 3f;
    [SerializeField, Min(0f)] private float maxDashDistance = 10f;
    [SerializeField, Min(0f)] private float dashLaneTolerance = 0.55f;
    [SerializeField, Min(0f)] private float dashMissRecoveryTime = 0.35f;

    private readonly DashGrabState dashGrabState = new DashGrabState();
    private float nextDashTime;

    public override void EnterBattleState()
    {
        TransitionToState(attackState);
    }

    public bool TryStartDashGrab(PlayerController player)
    {
        if (!canUseChargeSkill || rb == null || chargeSpeed <= 0f ||
            player == null || targetPoint != player.transform || player.isDead ||
            player.isCaptured || player.isDashAttack ||
            Time.time < nextCatchTime || Time.time < nextDashTime)
            return false;

        Vector2 offset = (Vector2)player.transform.position - rb.position;
        float horizontalDistance = Mathf.Abs(offset.x);
        if (horizontalDistance <= attackRange || horizontalDistance < minDashDistance ||
            horizontalDistance > maxDashDistance || Mathf.Abs(offset.y) > dashLaneTolerance)
            return false;

        TransitionToState(dashGrabState);
        return true;
    }

    public override void Update()
    {
        if (dashGrabState.IsActive && (isDead ||
            (!isCatching && RoomGenerator.instance != null && RoomGenerator.instance.gameOver)))
            dashGrabState.ExitState(this);

        base.Update();
    }

    private void FixedUpdate()
    {
        if (!isAVGPaused && !isDead && !isHurt && !isCatching &&
            (RoomGenerator.instance == null || !RoomGenerator.instance.gameOver))
            dashGrabState.Move(this);
    }

    private sealed class DashGrabState : EnemyBaseState
    {
        private enum Phase { Inactive, Ready, Move, Throw, Miss }

        private readonly RaycastHit2D[] wallHits = new RaycastHit2D[8];
        private ContactFilter2D wallFilter;
        private Phase phase;
        private PlayerController player;
        private Vector2 direction;
        private float timer;
        private float distanceLeft;

        public bool IsActive => phase != Phase.Inactive;

        public override void EnterState(EnemyController enemy)
        {
            var grappler = (Enemy_3)enemy;
            player = enemy.targetPoint.GetComponent<PlayerController>();
            direction = Vector2.right * Mathf.Sign(enemy.targetPoint.position.x - enemy.rb.position.x);
            distanceLeft = grappler.maxDashDistance;
            timer = enemy.chargeReadyTime;
            phase = Phase.Ready;
            wallFilter.SetLayerMask(enemy.hurtWallMask);
            wallFilter.useTriggers = false;

            grappler.nextDashTime = Time.time + enemy.chargeCooldown;
            enemy.CleanState();
            enemy.rb.velocity = Vector2.zero;
            enemy.FaceToPosition(enemy.transform.position + (Vector3)direction);
            enemy.SetStateColor(enemy.ChargeSkillColor);
            enemy.SetAnimState(4);
            enemy.anim.SetInteger("skillState", 1);
        }

        public override void OnUpdate(EnemyController enemy)
        {
            if (phase != Phase.Ready && phase != Phase.Miss)
                return;

            timer -= Time.deltaTime;
            if (timer > 0f)
                return;

            if (phase == Phase.Miss)
            {
                enemy.EnterBattleState();
                return;
            }

            phase = Phase.Move;
            enemy.anim.SetInteger("skillState", 2);
        }

        public void Move(Enemy_3 enemy)
        {
            if (phase != Phase.Move)
                return;

            Vector2 position = enemy.rb.position;
            float step = Mathf.Min(distanceLeft, enemy.chargeSpeed * Time.fixedDeltaTime);
            float grabReach = enemy.chargeStopDistance;
            float wallDistance = float.PositiveInfinity;
            int hitCount = enemy.rb.Cast(direction, wallFilter, wallHits, step + grabReach);
            for (int i = 0; i < hitCount; i++)
                wallDistance = Mathf.Min(wallDistance, wallHits[i].distance);

            if (player != null && !player.isDead && !player.isCaptured && !player.isDashAttack)
            {
                Vector2 offset = (Vector2)player.transform.position - position;
                float forwardDistance = Vector2.Dot(offset, direction);
                if (forwardDistance >= 0f && forwardDistance <= step + grabReach &&
                    forwardDistance < wallDistance && Mathf.Abs(offset.y) <= enemy.dashLaneTolerance)
                {
                    enemy.StartCatchPlayer(player);
                    if (enemy.isCatching)
                    {
                        phase = Phase.Throw;
                        enemy.rb.velocity = Vector2.zero;
                        enemy.anim.ResetTrigger("catchSuccess");
                        enemy.anim.SetInteger("skillState", 3);
                        return;
                    }
                }
            }

            if (step <= 0f || wallDistance <= step)
            {
                phase = Phase.Miss;
                timer = enemy.dashMissRecoveryTime;
                enemy.rb.velocity = Vector2.zero;
                enemy.anim.SetInteger("skillState", 0);
                enemy.SetAnimState(0);
                return;
            }

            enemy.rb.MovePosition(position + direction * step);
            distanceLeft -= step;
        }

        public override void ExitState(EnemyController enemy)
        {
            phase = Phase.Inactive;
            player = null;
            enemy.anim.SetInteger("skillState", 0);
            enemy.SetAnimState(0);
            enemy.StopMove();
        }
    }
}
