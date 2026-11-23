using UnityEngine;

public class Enemy_5 : EnemyController
{
    [Header("高速绕圈")]
    [SerializeField, Min(0.1f)] private float loopSpeed = 6f;
    [SerializeField, Min(0f)] private float playerAvoidDistance = 2f;
    [SerializeField, Min(0f)] private float minJumpInterval = 4f;
    [SerializeField, Min(0f)] private float maxJumpInterval = 7f;

    private Transform[] loopPoints = System.Array.Empty<Transform>();
    private readonly LoopState loopState = new LoopState();
    private readonly DownedPlayerCatchState downedPlayerCatchState = new DownedPlayerCatchState();
    private readonly JumpStrikeRecoveryState jumpStrikeRecoveryState = new JumpStrikeRecoveryState();

    [Header("跳跃落地技能")]
    public bool canUseJumpStrike = true;

    [Tooltip("瞄准玩家的持续时间")]
    public float jumpAimTime = 1f;

    [SerializeField, Min(0f), Tooltip("落地攻击结束后，停在原地且可被攻击的时间。")]
    private float jumpRecoveryTime = 1f;

    [Tooltip("瞬移落点和玩家之间的偏移")]
    public Vector2 landingOffset;

    [Tooltip("瞄准标记")]
    public Transform jumpAimTarget;


    [Tooltip("落地范围攻击碰撞体")]
    public GameObject jumpStrikeCollider;


    [HideInInspector]
    public Vector2 jumpStrikeTargetPos;

    public JumpStrikeSkillState jumpStrikeState =
        new JumpStrikeSkillState();


    public bool isJumpStrikeUntargetable;//跳跃攻击期间的无敌状态
    public override bool IgnoreIncomingDamage()
    {
        return isJumpStrikeUntargetable;
    }

    public void SetLoopPoints(Transform[] points)
    {
        loopPoints = points ?? System.Array.Empty<Transform>();
    }

    protected override void Start()
    {
        if (loopPoints.Length < 2)
            Debug.LogWarning("Enemy_5 需要在区域生成器上配置绕圈点。", this);

        EnterBattleState();
    }

    public override void EnterBattleState()
    {
        TransitionToState(loopState);
    }

    public void RecoverFromJumpStrike()
    {
        TransitionToState(jumpStrikeRecoveryState);
    }

    public override void Update()
    {
        if (isDead || (RoomGenerator.instance != null && RoomGenerator.instance.gameOver))
            jumpStrikeState.ExitState(this);

        base.Update();
    }

    protected override void ResumeAfterInterruption()
    {
        EnterBattleState();
    }

    public override void StartCatchPlayer(PlayerController player)
    {
        if (!downedPlayerCatchState.CanGrab || !CanPursueCatch(player) || targetPoint != player.transform)
            return;

        base.StartCatchPlayer(player);
        downedPlayerCatchState.GrabSucceeded = isCatching;
    }

    private bool CanPursueCatch(PlayerController player)
    {
        return player != null && !player.isDead && !player.isCaptured && !player.isDashAttack &&
            player.isHurt && player.hurtPhase == PlayerController.HurtPhase.Down &&
            Time.time >= nextCatchTime;
    }

    private sealed class JumpStrikeRecoveryState : EnemyBaseState
    {
        private float remainingTime;

        public override void EnterState(EnemyController enemy)
        {
            var enemy5 = (Enemy_5)enemy;
            remainingTime = enemy5.jumpRecoveryTime;
            enemy.targetPoint = null;
            enemy.CleanState();
        }

        public override void OnUpdate(EnemyController enemy)
        {
            remainingTime -= Time.deltaTime;
            if (remainingTime <= 0f)
                enemy.EnterBattleState();
        }
    }

    private sealed class DownedPlayerCatchState : EnemyBaseState
    {
        private static readonly int HoldAnimation = Animator.StringToHash("lewdmove");
        private PlayerController player;
        public bool CanGrab { get; private set; }
        public bool GrabSucceeded { get; set; }

        public override void EnterState(EnemyController enemy)
        {
            enemy.SetStateColor(enemy.attackColor);
            GrabSucceeded = false;
            player = enemy.targetPoint.GetComponent<PlayerController>();
            OnUpdate(enemy);
            if (enemy.aiPath != null && enemy.aiPath.canMove && enemy.enemyTarget != null)
            {
                enemy.aiPath.destination = enemy.enemyTarget.position;
                enemy.aiPath.SearchPath();
            }
        }

        public override void OnUpdate(EnemyController enemy)
        {
            var enemy5 = (Enemy_5)enemy;
            if (GrabSucceeded)
            {
                // 保持攻击动画参数，等 Animator 处理玩家的挣脱触发器后再跑圈。
                int attackLayer = enemy.anim.GetLayerIndex("Attack Layer");
                if (attackLayer >= 0 && enemy.anim.GetCurrentAnimatorStateInfo(attackLayer).shortNameHash == HoldAnimation)
                    return;

                enemy5.EnterBattleState();
                return;
            }

            if (!enemy5.CanPursueCatch(player))
            {
                enemy5.EnterBattleState();
                return;
            }

            CanGrab = Vector2.Distance(enemy.transform.position, player.transform.position) <= enemy.attackRange;
            if (CanGrab)
            {
                enemy.SetAnimState(3);
                enemy.AttackAction();
            }
            else if (enemy.aiPath != null && enemy.enemyTarget != null)
            {
                enemy.SetAnimState(2);
                enemy.MoveToTarget();
                enemy.aiPath.maxSpeed = enemy5.loopSpeed;
            }
        }

        public override void ExitState(EnemyController enemy)
        {
            player = null;
            CanGrab = false;
            enemy.anim.ResetTrigger("catch");
            if (enemy.Catch_Collider != null)
                enemy.Catch_Collider.SetActive(false);
            if (enemy.catchCollider != null)
                enemy.catchCollider.ResetCatch();
        }
    }

    private sealed class LoopState : EnemyBaseState
    {
        private int pointIndex;
        private float jumpTimer;
        private float retargetTimer;

        public override void EnterState(EnemyController enemy)
        {
            var enemy5 = (Enemy_5)enemy;
            enemy.CleanState();
            enemy.targetPoint = null;
            enemy.SetStateColor(enemy.patrolColor);
            jumpTimer = Random.Range(enemy5.minJumpInterval,
                Mathf.Max(enemy5.minJumpInterval, enemy5.maxJumpInterval));
            retargetTimer = 0f;
            pointIndex = -1;
        }

        public override void OnUpdate(EnemyController enemy)
        {
            var enemy5 = (Enemy_5)enemy;
            PlayerController player = RoomGenerator.instance != null ? RoomGenerator.instance.player : null;
            if (enemy5.CanPursueCatch(player))
            {
                enemy.targetPoint = player.transform;
                enemy.TransitionToState(enemy5.downedPlayerCatchState);
                return;
            }

            jumpTimer -= Time.deltaTime;
            if (jumpTimer <= 0f && enemy5.canUseJumpStrike &&
                player != null && !player.isDead && !player.isCaptured)
            {
                enemy.targetPoint = player.transform;
                enemy.TransitionToState(enemy5.jumpStrikeState);
                return;
            }

            Transform[] points = enemy5.loopPoints;
            if (points.Length == 0 || enemy.aiPath == null || enemy.enemyTarget == null)
                return;

            Vector2 position = enemy.transform.position;
            Transform currentPoint = pointIndex >= 0 ? points[pointIndex] : null;
            retargetTimer -= Time.deltaTime;
            bool arrived = currentPoint != null &&
                (Vector2.Distance(position, currentPoint.position) <= enemy.arriveDistance ||
                 (!enemy.aiPath.pathPending && enemy.aiPath.reachedEndOfPath && retargetTimer <= 0f));
            bool playerBlocksRoute = currentPoint != null && player != null && retargetTimer <= 0f &&
                Vector2.Distance(position, player.transform.position) <= enemy5.playerAvoidDistance &&
                Vector2.Dot((Vector2)currentPoint.position - position,
                    (Vector2)player.transform.position - position) > 0f;

            if (currentPoint == null || arrived || playerBlocksRoute)
            {
                int preferredIndex = pointIndex < 0 ? 0 : (pointIndex + 1) % points.Length;
                pointIndex = SelectPoint(points, preferredIndex, position, player,
                    enemy5.playerAvoidDistance, enemy.arriveDistance, pointIndex < 0 || playerBlocksRoute);
                if (pointIndex < 0)
                {
                    enemy.StopMove();
                    enemy.SetAnimState(0);
                    return;
                }

                enemy.enemyTarget.position = points[pointIndex].position;
                enemy.aiPath.destination = enemy.enemyTarget.position;
                enemy.aiPath.SearchPath();
                retargetTimer = 0.35f;
            }

            enemy.aiPath.maxSpeed = enemy5.loopSpeed;
            enemy.aiPath.canMove = true;
            enemy.animState = 1;
            float horizontalVelocity = enemy.aiPath.desiredVelocity.x;
            if (Mathf.Abs(horizontalVelocity) > 0.05f)
                enemy.FaceToPosition(enemy.transform.position + Vector3.right * horizontalVelocity);
        }

        private static int SelectPoint(Transform[] points, int preferredIndex, Vector2 position,
            PlayerController player, float avoidDistance, float arriveDistance, bool rejoin)
        {
            int bestIndex = -1;
            float bestScore = float.NegativeInfinity;
            for (int offset = 0; offset < points.Length; offset++)
            {
                int index = (preferredIndex + offset) % points.Length;
                if (points[index] == null)
                    continue;

                Vector2 leg = (Vector2)points[index].position - position;
                if (leg.sqrMagnitude <= arriveDistance * arriveDistance)
                    continue;

                float clearance = avoidDistance;
                if (player != null)
                {
                    Vector2 toPlayer = (Vector2)player.transform.position - position;
                    float projection = Vector2.Dot(toPlayer, leg);
                    float alongLeg = Mathf.Clamp01(projection / leg.sqrMagnitude);
                    clearance = Mathf.Min(avoidDistance, (toPlayer - alongLeg * leg).magnitude);

                    // 从玩家身边起跑时，优先朝远离玩家的方向离开。
                    float proximity = Mathf.Max(0f, avoidDistance - toPlayer.magnitude);
                    clearance -= proximity * Vector2.Dot(toPlayer.normalized, leg.normalized);
                }

                // 优先走安全的下一段，重新加入路线时优先选近处的点。
                float score = clearance - (rejoin ? leg.magnitude : offset) * 0.01f;
                if (score > bestScore)
                {
                    bestScore = score;
                    bestIndex = index;
                }
            }

            return bestIndex;
        }
    }
}
