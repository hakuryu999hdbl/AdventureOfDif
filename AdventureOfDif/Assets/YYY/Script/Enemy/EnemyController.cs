using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyController : MonoBehaviour
{
    [Header("A星寻路")]
    public AIPath aiPath;
    public Transform enemyTarget;
    public float arriveDistance = 0.2f;

    [Header("巡逻点")]
    private Transform currentPatrolPoint;
    public List<Transform> patrolPoints = new List<Transform>();



    [Header("动画状态")]
    EnemyBaseState currentState;//当前状态
    public Animator anim;
    public int animState;
    public CharacterSkin characterSkin;

    [Header("基础属性")]
    public Rigidbody2D rb;
    public bool isDead = false;
    public bool isHurt = false;//为了受伤的时候屏蔽Update

    [Header("敌人攻击")]
    public GameObject attack_Collider_1;
    public GameObject attack_Collider_2;
    public float attackRate;//攻击冷却
    public float attackRange;//攻击范围
    float nextAttack = 0;

    public List<Transform> attackList = new List<Transform>();


    public PatrolState patrolState = new PatrolState();//巡逻状态
    public AttackState attackState = new AttackState();//攻击状态
    public HitState hitState = new HitState();//受击状态
    public ChargeSkillState chargeSkillState = new ChargeSkillState();//冲刺攻击状态
    public AimThrowSkillState aimThrowSkillState = new AimThrowSkillState();//瞄准攻击状态
    public BlockState blockState = new BlockState();//防御状态


    public virtual void Init()
    {
        rb = GetComponent<Rigidbody2D>();

        if (aiPath == null)
            aiPath = GetComponent<AIPath>();

        if (enemyTarget == null)
        {
            var t = transform.parent.Find("Enemy_Target");
            if (t != null) enemyTarget = t;
        }

    }//敌人子类会各自在开始的时候收进父级不需要的东西（虚类）

    public virtual void EnterBattleState()
    {
        TransitionToState(attackState);
    }//敌人子类进入战斗状态

    private void Awake()
    {
        Init();
    }

    void Start()
    {


        TransitionToState(patrolState);//一开始进入巡逻状态

    }

    public virtual void Update()
    {

        anim.SetBool("dead", isDead);

        if (isDead)
        {
            return;
        }


        if (isCatching)
        {
            StopMove();

            return;
        }//抓住玩家期间禁止移动



        if (RoomGenerator.instance != null && RoomGenerator.instance.gameOver)
        {
            attackList.Clear();
            targetPoint = null;
            CleanState();//玩家死后强制停战
            return;
        }

        currentState.OnUpdate(this);//每帧执行状态
        anim.SetInteger("state", animState);


        //当这些动画在播放的时候玩家不能移动
        //AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
        //
        //if (state.IsName("hurt_1") ||
        //    state.IsName("hurt_2")          
        //    )
        //{
        //    StopMove();
        //
        //
        //}
        //else
        //{
        //    ResumeAI();
        //}


    }




    public void TransitionToState(EnemyBaseState nextState)
    {
        currentState?.ExitState(this);

        currentState = nextState;

        currentState.EnterState(this);
    }//切换状态

    public void SetAnimState(int state)
    {
        animState = state;
        anim.SetInteger("state", state);
    }//动画器层的切换统一入口
    public void CleanState()
    {
        SetAnimState(0);
        anim.SetInteger("state", 0);
        anim.ResetTrigger("attack");
        anim.ResetTrigger("catchSuccess");


        // ★清除抓取尝试窗口
        if (Catch_Collider != null)
            Catch_Collider.SetActive(false);

        if (catchCollider != null)
            catchCollider.ResetCatch();


        StopMove();


    }//清理状态的统一入口


    /// <summary>
    /// 攻击状态
    /// </summary>
    #region
    [Header("攻击状态")]
    public Transform targetPoint;
    public void MoveToTarget()
    {

        //if (targetPoint == null || enemyTarget == null) return;

        enemyTarget.position = targetPoint.position;

        ResumeAI();

        FilpDirection();
        //Debug.Log("冲向目标！");

    }//锁定目标







    public void AttackAction()
    {
        //Debug.Log("攻击！");

        if (targetPoint == null) return;

        float distance = Vector2.Distance(transform.position, targetPoint.position);

        if (distance <= attackRange)
        {
            StopMove();//攻击近敌停
            FilpDirection();

            if (Time.time > nextAttack)
            {
                //Debug.Log("普通攻击");
                nextAttack = Time.time + attackRate;


                PlayerController player = RoomGenerator.instance.player;


                // 玩家死亡：只允许抓取
                if (player.isDead)
                {
                    anim.SetTrigger("catch");
                    return;
                }

                // 玩家倒地：只允许抓取
                if (player.isHurt &&
                    player.hurtPhase == PlayerController.HurtPhase.Down)
                {
                    anim.SetTrigger("catch");
                    return;
                }

                // 其他受击过程：暂时停手
                if (player.isHurt)
                {
                    return;
                }

                // 正常状态：普通攻击
                anim.SetInteger(
                    "attackType",
                    Random.Range(1, 3)
                );

                anim.SetTrigger("attack");




                // // 死亡：完全停止攻击
                // if (player.isDead)
                // {
                //     SetAnimState(0);
                //     TransitionToState(patrolState);
                //     return;
                // }
                //
                // // 正在受击/飞行/爬起：不能攻击
                // // 但是 Down 是特殊情况，允许抓取
                // if (player.isHurt &&
                //     player.hurtPhase != PlayerController.HurtPhase.Down)
                // {
                //     SetAnimState(0);
                //     TransitionToState(patrolState);
                //     return;
                // }
                //
                // // 玩家倒地：所有敌人统一尝试抓取
                // if (player.hurtPhase == PlayerController.HurtPhase.Down)
                // {
                //     anim.SetTrigger("catch");
                // }
                // else
                // {
                //     anim.SetInteger("attackType", Random.Range(1, 3));
                //     anim.SetTrigger("attack");
                // }

            }
        }


    }//攻击

    public void FilpDirection()
    {
        if (transform.position.x < targetPoint.position.x)
        {
            transform.localScale = new Vector3(1, 1, 1);

        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);

        }

    }//反转：追逐目标

    public Transform GetNearestTarget(List<Transform> list)
    {
        Transform nearest = null;
        float minDistance = Mathf.Infinity;

        foreach (Transform t in list)
        {
            if (t == null) continue;

            float distance = Vector2.Distance(transform.position, t.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = t;
            }
        }

        return nearest;
    }


    [Header("远程攻击")]
    public GameObject Obstacle_Attack;
    public Transform throwPoint;
    public float throwSpeedX = 4f;
    public float throwSpeedY = 2f;

    public void ThrowHeldObject()
    {
        if (Obstacle_Attack == null) return;

        Vector3 spawnPos = throwPoint != null ? throwPoint.position : transform.position;

        GameObject obj = Instantiate(
            Obstacle_Attack,
            spawnPos,
            Quaternion.identity
        );

        float dir;

        if (targetPoint != null)
            dir = targetPoint.position.x > transform.position.x ? 1f : -1f;
        else
            dir = transform.localScale.x >= 0 ? 1f : -1f;

        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.simulated = true;
            rb.velocity = new Vector2(throwSpeedX * dir, throwSpeedY);
        }

        ThrowHeldObject throwObj = obj.GetComponent<ThrowHeldObject>();

        if (throwObj != null)
        {
            throwObj.Launch(GrabbableObject.GrabbableType.Tanker);
        }

        Debug.Log("投掷攻击");
    }

    #endregion


    /// <summary>
    /// 冲刺技能
    /// </summary>
    #region
    [Header("冲刺技能")]
    public bool canUseChargeSkill = true;
    public float chargeReadyTime = 1f;
    public float chargeSpeed = 8f;
    public float chargeStopDistance = 0.15f;
    public float chargeCooldown = 5f;

    [HideInInspector] public float lastChargeTime;
    [HideInInspector] public Vector2 chargeTargetPos;

    public Animator aimUI;

    public void MoveToChargeTarget(Vector2 pos, float speed)
    {
        if (aiPath == null || enemyTarget == null) return;

        enemyTarget.position = pos;

        aiPath.canMove = true;
        aiPath.maxSpeed = speed;

        if (aiPath.desiredVelocity.x > 0.05f)
            transform.localScale = new Vector3(1, 1, 1);
        else if (aiPath.desiredVelocity.x < -0.05f)
            transform.localScale = new Vector3(-1, 1, 1);
    }
    public void ChargeSkillOver()
    {
        lastChargeTime = Time.time;

        StopMove();

        anim.SetInteger("skillState", 0);

        targetPoint = null;
        //attackList.Clear();//万一玩家还在附近就又需要进入了

        TransitionToState(patrolState);

        Debug.Log("出拳结束");
    }

    #endregion


    /// <summary>
    /// 投掷技能
    /// </summary>
    #region
    [Header("瞄准投掷技能")]
    public Transform throwAimTarget;// 瞄准物体
    public GameObject throwExplosionPrefab;//爆炸

    public float throwAimTime = 1f;
    public Vector2 throwTargetPos;

    public EnemyThrowObject throwObject;//投掷物

    public void AimThrowSpawnExplosion()
    {
        if (throwExplosionPrefab != null)
        {
            //GameObject effect = Instantiate(
            //    throwExplosionPrefab,
            //    throwTargetPos,
            //    Quaternion.identity
            //);
            //
            //Destroy(effect, 1.2f);


            throwObject.Launch(throwPoint.position, throwTargetPos);
        }
    }

    public void AimThrowStartLaugh()
    {
        anim.SetInteger("skillState", 3);
    }

    public void AimThrowOver()
    {
        StopMove();

        anim.SetInteger("skillState", 0);

        if (throwAimTarget != null)
            throwAimTarget.gameObject.SetActive(false);//瞄准消失

        targetPoint = null;

        TransitionToState(patrolState);
    }


    public void FaceToPosition(Vector3 targetPos)
    {
        float dir = targetPos.x - transform.position.x;

        if (dir > 0)
            transform.localScale = Vector3.one;
        else if (dir < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }//因该是有一样方法的，可以把写重复的地方引导到这里

    #endregion



    /// <summary>
    /// 抓取技能
    /// </summary>
    #region
    [Header("抓取技能")]
    public bool isCatching;//在整个抓取过程中挡住FSM
    public PlayerController capturedPlayer;
    public Catch catchCollider;

    [Header("抓取冷却")]
    public float catchCooldown = 1.0f;
    public float nextCatchTime;

    public GameObject Catch_Collider;//让动画产生，防止bug

    public void StartCatchPlayer(PlayerController player)
    {

        if (isHurt) return;
        if (isDead) return;
        if (isCatching) return;
        if (capturedPlayer != null) return;
        if (Time.time < nextCatchTime) return;
        if (player == null ||
           // player.isDead ||
           // player.isHurt ||
            player.isCaptured ||
            player.isDashAttack)
        {
            return;
        }

        // ===== 抓取正式成立 =====

        isCatching = true;
        capturedPlayer = player;



        StopMove();


        // 确保其他攻击碰撞体不会残留

        if (Catch_Collider != null)
            Catch_Collider.SetActive(false);
            catchCollider.ResetCatch();

        player.EnterCapturedState();//玩家进入透明

        frameEvents._Attack_pick();//抓取声音


        // ★关键：
        // 从 attack_ready 立即切到 attack_throw/Lewdmove
        anim.SetTrigger("catchSuccess");

        Debug.Log("抓住玩家：" + capturedPlayer.name);
    }

    public void ThrowCapturedPlayer()
    {
        // 不是有效抓取状态时，旧动画事件直接无视
        if (!isCatching || capturedPlayer == null)
        {
            //CancelCatch(false);
            return;
        }

        Debug.Log("投出");

        PlayerController player = capturedPlayer;
        capturedPlayer = null;

        float dir = transform.localScale.x > 0 ? 1f : -1f;

        // 先结束敌人抓取状态，再恢复玩家
        //isCatching = false;
        nextCatchTime = Time.time + catchCooldown;

        if (catchCollider != null)
            catchCollider.ResetCatch();

        if (Catch_Collider != null)
            Catch_Collider.SetActive(false);

        player.ExitCapturedState(new Vector2(6f * dir, 4f));

        //
        //SetAnimState(0);
        //TransitionToState(patrolState);
    }

    public void CatchAnimationOver()
    {
        if (!isCatching)
            return;

        isCatching = false;
        capturedPlayer = null;

        if (Catch_Collider != null)
            Catch_Collider.SetActive(false);

        if (catchCollider != null)
            catchCollider.ResetCatch();

        SetAnimState(0);

        // 玩家还在敌人索敌范围
        if (attackList != null && attackList.Count > 0)
        {
            TransitionToState(attackState);
        }
        else
        {
            TransitionToState(patrolState);
        }

        Debug.Log("抓取动画结束");
    }






    public void StartPlayerStruggle()
    {
        if (!isCatching || capturedPlayer == null)
            return;

        capturedPlayer.StartStruggle(this);
    }//开启挣扎


    public void BreakFreeFromPlayer(PlayerController player)
    {
        if (!isCatching)
            return;

        if (capturedPlayer != player)
            return;

        // 先解除双方关系
        capturedPlayer = null;

        isCatching = false;
        nextCatchTime = Time.time + catchCooldown;

        if (catchCollider != null)
            catchCollider.ResetCatch();

        if (Catch_Collider != null)
            Catch_Collider.SetActive(false);

        // 玩家结束挣扎状态
        player.EndStruggle();

        // 玩家恢复显示
        player.ExitCapturedState(Vector2.zero);


        // 玩家播放挣脱攻击动画
        player.playerAnimation.anim.SetTrigger("breakFree");

        // 敌人退出 lewdmove
        anim.SetTrigger("breakFree");

    }//玩家挣扎值满了后的挣脱







    #endregion


    /// <summary>
    /// 防御技能
    /// </summary>
    #region
    public virtual bool TryHandleIncomingAttack(Attack attack)
    {
        return false;
    }


    #endregion


    /// <summary>
    /// 巡逻状态
    /// </summary>
    #region
    [Header("自由巡逻")]
    public float minWalkTime = 1.5f;
    public float maxWalkTime = 4f;
    public float minIdleTime = 1f;
    public float maxIdleTime = 3f;

    [HideInInspector] public int patrolDir = 1;

    public void MovePatrol()
    {
        if (aiPath == null || enemyTarget == null || isHurt || isDead)
            return;

        aiPath.canMove = true;
        aiPath.maxSpeed = 1.5f;

        if (currentPatrolPoint == null || Vector2.Distance(transform.position, currentPatrolPoint.position) <= arriveDistance)
        {
            SetNewPatrolTarget();
        }

        // 根据移动方向翻面
        if (aiPath.desiredVelocity.x > 0.05f)
            transform.localScale = new Vector3(1, 1, 1);
        else if (aiPath.desiredVelocity.x < -0.05f)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    public void OnDamageOver()
    {
        Debug.Log("重置");

        isHurt = false;
        hurtPhase = HurtPhase.None;

        anim.SetBool("hit", false);
        anim.SetBool("down", false);
        anim.SetInteger("HitType", 0);

        ResetFakeHeight();
        ResetShadow();

        if (isDead) return;

        if (attackList.Count > 0)
            TransitionToState(attackState);
        else
            TransitionToState(patrolState);





    }//受伤后恢复

    public void ResumeAI()
    {
        aiPath.canMove = true;
        aiPath.maxSpeed = 1.5f;
    }//巡逻走
    public void StopMove()
    {
        if (aiPath == null) return;

        aiPath.canMove = false;
        aiPath.maxSpeed = 0.01f;
    }////巡逻停，死亡停，受伤停，攻击近敌停

    private void SetNewPatrolTarget()
    {
        if (AreaManager.Instance == null)
        {
            StopMove();//无巡逻目标控制器停
            return;
        }

        currentPatrolPoint = AreaManager.Instance.GetRandomPatrolPoint();

        if (currentPatrolPoint == null)
        {
            StopMove();//无巡逻目标停
            return;
        }

        enemyTarget.position = currentPatrolPoint.position;
    }//设置下一个巡逻目的地

    public void ForceResetPatrolTarget()
    {
        currentPatrolPoint = null;
    }//清空巡逻目标到达重置目的

    #endregion


    /// <summary>
    /// 2.5D受击假物理
    /// </summary>
    #region
    [Header("2.5D受击假物理")]
    [Tooltip("Spine或显示用身体根节点。假高度只移动它，不移动Enemy本体。")]
    public Transform bodyRoot;
    [Tooltip("地面水平击退速度衰减。越大越快停。")]
    public float hurtGroundFriction = 8f;
    [Tooltip("假重力。越大落地越快。")]
    public float fakeGravity = 20f;
    [Tooltip("落地后倒地/硬直追加时间。")]
    public float knockDownTime = 0.35f;
    [Tooltip("是否检测墙壁反弹。")]
    public bool useWallBounce = true;
    [Tooltip("墙壁/障碍层。建议填Map/Obstacle。")]
    public LayerMask hurtWallMask;
    [Tooltip("墙壁检测距离。")]
    public float wallCheckDistance = 0.25f;
    [Tooltip("撞墙反弹保留速度比例。")]
    public float wallBounceDamping = 0.45f;

    private Vector3 bodyRootStartLocalPos;
    private Vector2 hurtGroundVelocity;
    private float fakeHeight;
    private float fakeVerticalVelocity;
    private float hurtTimer;
    private float downTimer;
    private bool hasLanded;
    private bool hasHitWall;//撞墙触发声音

    public void StartHurtMotion(Attack attack)
    {
        if (attack == null) return;

        StopMove();
        isHurt = true;
        hasLanded = false;
        hasHitWall = false; // 新的一次受击允许播放一次撞墙声
        downTimer = 0f;

        float dir = transform.position.x >= attack.transform.position.x ? 1f : -1f;

        hurtGroundVelocity = new Vector2(dir * attack.knockbackX, 0f);
        fakeVerticalVelocity = attack.knockbackY;
        fakeHeight = 0.01f;
        hurtTimer = Mathf.Max(attack.hurtTime, 0.05f);

        if (attack.clearVelocity && rb != null)
            rb.velocity = Vector2.zero;

        if (bodyRoot != null)
            bodyRootStartLocalPos = bodyRoot.localPosition;

    }//受击飞起

    public bool UpdateHurtMotion()
    {
        StopMove();

        UpdateHurtGroundMove();

        UpdateAirSorting();

        hurtTimer -= Time.deltaTime;

        if (hurtPhase == HurtPhase.Hurt)
        {
            return hurtTimer <= 0f;
        }

        if (hurtPhase == HurtPhase.Fly)
        {
            UpdateFakeHeight();
            return false;
        }

        if (hurtPhase == HurtPhase.Down)
        {
            downTimer -= Time.deltaTime;

            if (downTimer <= 0f)
            {
                hurtPhase = HurtPhase.GetUp;

                if (characterSkin != null)
                    characterSkin.canAnimEndHurt = true;

                anim.SetInteger("HitType", 0);
                anim.SetBool("down", false);
            }

            return false;
        }

        if (hurtPhase == HurtPhase.GetUp)
        {
            return false;
        }

        return false;
    }//这个是持续检测，目前好像只有受伤状态下会这样

    private void UpdateHurtGroundMove()
    {
        if (hurtGroundVelocity.sqrMagnitude <= 0.0001f) return;

        Vector2 move = hurtGroundVelocity * Time.deltaTime;

        if (useWallBounce && hurtWallMask.value != 0)
        {
            Vector2 dir = hurtGroundVelocity.normalized;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, wallCheckDistance, hurtWallMask);//打到墙上反弹一点

            if (hit.collider != null)
            {
                // 每次受击最多播放一次撞墙声音
                if (!hasHitWall)
                {
                    hasHitWall = true;

                    frameEvents._Attack_hit();

                }



                hurtGroundVelocity.x = -hurtGroundVelocity.x * wallBounceDamping;
                move = hurtGroundVelocity * Time.deltaTime;
            }
        }

        transform.position += (Vector3)move;

        hurtGroundVelocity = Vector2.MoveTowards(
            hurtGroundVelocity,
            Vector2.zero,
            hurtGroundFriction * Time.deltaTime
        );
    }//横向击退

    [Header("空中排序")]
    public Renderer spineRenderer;
    public int groundOrder = 0;
    public int airOrder = 1;

    private void UpdateAirSorting()
    {
        if (spineRenderer == null) return;

        if (fakeHeight > 0.01f)
            spineRenderer.sortingOrder = airOrder;
        else
            spineRenderer.sortingOrder = groundOrder;
    }//空中排序

    private void UpdateFakeHeight()
    {
        if (bodyRoot == null)
        {
            fakeHeight = 0f;
            fakeVerticalVelocity = 0f;
            return;
        }

        fakeVerticalVelocity -= fakeGravity * Time.deltaTime;
        fakeHeight += fakeVerticalVelocity * Time.deltaTime;

        if (fakeHeight <= 0f)
        {
            fakeHeight = 0f;
            fakeVerticalVelocity = 0f;

            if (hurtPhase == HurtPhase.Fly)
            {
                hurtPhase = HurtPhase.Down;
                downTimer = knockDownTime;

                anim.SetBool("down", true);
                //AudioManager.Instance.PlayFX(AudioManager.Instance.SE_falldown);//落地声
                frameEvents._SE_falldown();//落地声
                Debug.Log("落地，进入倒地状态");
            }
        }

        Vector3 localPos = bodyRootStartLocalPos;
        localPos.y += fakeHeight;
        bodyRoot.localPosition = localPos;

        UpdateShadow();
    }//模拟高度

    private void ResetFakeHeight()
    {
        hurtGroundVelocity = Vector2.zero;
        fakeHeight = 0f;
        fakeVerticalVelocity = 0f;
        hurtTimer = 0f;
        downTimer = 0f;
        hasLanded = false;

        if (bodyRoot != null)
            bodyRoot.localPosition = bodyRootStartLocalPos;

        if (spineRenderer != null)
            spineRenderer.sortingOrder = groundOrder;
    }//重置模拟高度


    [Header("影子控制")]
    public Transform shadow;
    public Vector2 shadowBaseScale = Vector2.one;
    public float shadowMinScale = 0.3f;
    public float maxJumpHeight = 3f;

    void UpdateShadow()
    {
        if (shadow == null)
            return;

        // 跟随敌人地面位置
        shadow.position = new Vector3(
            transform.position.x,
            shadow.position.y,
            transform.position.z
        );

        // 根据高度缩放
        float t = Mathf.Clamp01(fakeHeight / maxJumpHeight);

        float scale = Mathf.Lerp(
            1f,
            shadowMinScale,
            t
        );

        shadow.localScale = shadowBaseScale * scale;

        SpriteRenderer sr = shadow.GetComponent<SpriteRenderer>();

        if (sr != null)
        {
            Color c = sr.color;

            c.a = Mathf.Lerp(
                1f,
                0.5f,
                t
            );

            sr.color = c;
        }
    }

    private void ResetShadow()
    {
        if (shadow != null)
        {
            shadow.localScale = shadowBaseScale;

            SpriteRenderer sr = shadow.GetComponent<SpriteRenderer>();

            if (sr != null)
            {
                Color c = sr.color;
                c.a = 1f;
                sr.color = c;
            }
        }
    }

    #endregion


    /// <summary>
    /// 受伤死亡
    /// </summary>
    #region
    [Header("受伤死亡")]
    public GameObject Effect_Blood;//受伤特效
    public GameObject Strike_Effect;//剑光特效
    public GameObject Hit_Effect;//打击特效
    [Header("主动触发声音")]
    public FrameEvents frameEvents;
    public Character character;//主要是非Attack类直接回血用

    //受伤枚举
    public enum HurtPhase
    {
        None,
        Hurt,
        Fly,
        Down,
        GetUp
    }

    public HurtPhase hurtPhase;


    public void OnTakeDamage(Attack attack)
    {
        if (isDead) return;
        if (isHurt) return; // 受击流程中不再吃新攻击
        if (attack == null) return;



        // 抓取成功之后暂时不允许受伤
        if (isCatching || capturedPlayer != null)
            return;

        //isCatching = false;
        //capturedPlayer = null;
        //nextCatchTime = Time.time + catchCooldown;





       
        CleanState(); // 受击瞬间强制退出攻击动画层





        switch (attack.hitEffectType)
        {
            case 0:
                // 打击特效
                Hit_Effect.SetActive(true);
                frameEvents._Attack_hit();//击打声音
                break;

            case 1:
                // 斩击特效
                Strike_Effect.transform.localRotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(-45f, 45f));
                Strike_Effect.SetActive(true);
                break;
        }


        characterSkin.FlashRed();//受伤闪红



        //一旦受伤立刻把Attack的根物体的character所在物体立为目标
        Character attackerCharacter = attack.GetComponentInParent<Character>();

        if (attackerCharacter != null)
        {
            Transform attacker = attackerCharacter.transform;

            targetPoint = attacker;

            if (!attackList.Contains(attacker))
            {
                attackList.Add(attacker);
            }

            //TransitionToState(attackState);//受击时只记录攻击者，不切 AttackState
        }


        anim.SetBool("hit", true);





        if (attack.knockbackY > 0)
        {
            characterSkin.canAnimEndHurt = false;//如果是击飞，由落地控制离开受伤状态
            anim.SetInteger("HitType", 3);

            hurtPhase = HurtPhase.Fly;

        }
        else
        {
            characterSkin.canAnimEndHurt = true;//如果是击退，由动画结束控制离开受伤状态
            anim.SetInteger("HitType", Random.Range(1, 3));

            hurtPhase = HurtPhase.Hurt;

        }




        PlayBloodEffect();

        //isHurt = true;
        StartHurtMotion(attack);
        TransitionToState(hitState);//进入受击状态

        RoomGenerator.instance.player.ChangeCritical(200);//每一把受击都增加玩家爆气值
    }


    void PlayBloodEffect()
    {
        GameObject blood = Instantiate(
            Effect_Blood,
            transform.position,
            Quaternion.identity
        );

        frameEvents._Attack_blood();


        Destroy(blood, 1f); // 1秒后销毁
    }






    public void OnDie()
    {
        if (isDead) return;

        PlayBloodEffect();

        isDead = true;

        StopMove();//死亡停


        // 关闭所有攻击碰撞体
        if (attack_Collider_1 != null)
            attack_Collider_1.SetActive(false);

        if (attack_Collider_2 != null)
            attack_Collider_2.SetActive(false);

        if (Catch_Collider != null)
            Catch_Collider.SetActive(false);

        // 强制清理受击、倒地和攻击参数
        anim.ResetTrigger("attack");

        anim.SetBool("hit", false);
        anim.SetBool("down", false);

        anim.SetInteger("HitType", 0);
        anim.SetInteger("state", 0);
        anim.SetInteger("skillState", 0);

        // 最后再打开死亡
        anim.SetBool("dead", true);




        SetStateColor(deadColor);



        //给玩家加钱
        BalanceManager.instance.ChangeMoney(Random.Range(5, 11), false);//更新钱

    }
    public GameObject Enemy_All;
    public void DestroyEnemy()
    {
        Destroy(Enemy_All);
    }




    public virtual bool IgnoreIncomingDamage()
    {
        return false;
    }//无敌状态


    #endregion



    [Header("状态显示器")]
    public SpriteRenderer stateViewer;

    public Color patrolColor = Color.green;
    public Color attackColor = Color.red;
    public Color hitColor = Color.yellow;
    public Color ChargeSkillColor = new Color(1f, 0.6f, 0.7f);//粉色
    public Color AimThrowSkillColor = Color.cyan;
    public Color blockColor = new Color(1f, 0.5f, 0.4f);// 珊瑚橙
    public Color JumpStrikeColor = new Color(0.5f, 0.9f, 0.6f);  // 薄荷绿
    public Color deadColor = Color.gray;

    public void SetStateColor(Color color)
    {
        if (stateViewer != null)
            stateViewer.color = color;
    }
}
