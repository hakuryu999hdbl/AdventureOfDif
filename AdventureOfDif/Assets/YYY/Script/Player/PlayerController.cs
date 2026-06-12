using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    [Header("基础属性")]
    public CharacterSkin characterSkin;

    public Rigidbody2D rb;
    public float speed;


    [Header("死亡判定")]
    public PlayerAnimation playerAnimation;

    [Header("冲刺攻击")]
    public float dashAttackSpeed = 2f;
    public float dashAttackMoveTime = 1.0f;

    public LayerMask mapLayer;

    private Vector2 dashDir;
    private Vector2 lastMoveDir;

    [Header("状态")]
    public bool isAttack = false;//攻击的时候不能XXX
    public bool isDashAttack = false;//冲刺攻击
    public bool isWalk = false;
    public bool isHurt = false;
    public bool isDead = false;


    void Start()
    {
        //character = GetComponent<Character>();
        rb = GetComponent<Rigidbody2D>();

    }


    public void Update()
    {

        inputDirection = inputControl.Gameplay.Move.ReadValue<Vector2>();


    }//输入用Update（听）

    public void FixedUpdate()
    {
        if (isDead)
        {
            rb.velocity = Vector2.zero;
            return;
        }//死亡后不能滑行


        if (isHurt)
        {
            rb.velocity = Vector2.zero;

            if (UpdateHurtMotion())
            {
                OnDamageOver();
            }

            return;
        }


        if (!isHurt && !isAttack)
        {
            Move();
        }
        else if (isAttack && !isDashAttack)
        {
            rb.velocity = Vector2.zero;//攻击的时候刚体清零
        }


        CheckState();

    }//每帧执行动作用FixedUpdate（做）





    private void Move()
    {

        rb.velocity = inputDirection.normalized * speed;


        if (inputDirection.sqrMagnitude > 0.01f)
        {
            lastMoveDir = inputDirection.normalized;
        }//记录最后方向，冲刺攻击用


        if (inputDirection.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (inputDirection.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }


    void CheckState()
    {

    }


    public void OnDamageOver()
    {
        isHurt = false;
        hurtPhase = HurtPhase.None;

        playerAnimation.EndHurt();

        ResetFakeHeight();
        ResetShadow();

        RedScreen.SetActive(false);

    }//受伤后恢复


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

    public void StartHurtMotion(Attack attack)
    {
        if (attack == null) return;

        //StopMove();

        isHurt = true;
        hasLanded = false;
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
        UpdateAirSorting();
        UpdateHurtGroundMove();
        hurtTimer -= Time.deltaTime;

        if (hurtPhase == HurtPhase.Hurt)
            return hurtTimer <= 0f;

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
                characterSkin.canAnimEndHurt = true;

                playerAnimation.anim.SetInteger("hurtType", 0);
                playerAnimation.anim.SetBool("down", false);
            }

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

                playerAnimation.anim.SetBool("down", true);


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
    public float hurtForce;
    public GameObject RedScreen;
    public GameObject Effect_Blood;//受伤特效
    public GameObject Strike_Effect;//剑光特效
    public GameObject Hit_Effect;//打击特效


    [Header("主动触发声音")]
    public FrameEvents frameEvents;


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
        if (isHurt) return;
        if (attack == null) return;


        isAttack = false;
        isDashAttack = false;
        rb.velocity = Vector2.zero;




        Hit_Effect.SetActive(true);//TODO  之后伤害类型分开




        if (attack.knockbackY > 0)
        {
            characterSkin.canAnimEndHurt = false;//如果是击飞，由落地控制离开受伤状态
            hurtPhase = HurtPhase.Fly;
            playerAnimation.PlayFly();
        }
        else
        {
            characterSkin.canAnimEndHurt = true;//如果是击退，由动画结束控制离开受伤状态
            hurtPhase = HurtPhase.Hurt;
            playerAnimation.PlayHurt();
        }



        RedScreen.SetActive(true);
        PlayBloodEffect();

        StartHurtMotion(attack);
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



    public void PlayerDead()
    {

        PlayBloodEffect();

        isDead = true;
        inputControl.Gameplay.Disable();//通过直接禁用来做（但是防止4层多端输入，在上方也禁止）
    }


    #endregion

    /// <summary>
    /// 多端输入
    /// </summary>
    #region

    [Header("多端输入")]
    public PlayerInputControl inputControl;
    public Vector2 inputDirection;
    private void Awake()
    {
        inputControl = new PlayerInputControl();

        #region 强制走路

        inputControl.Gameplay.WalkButton.performed += ctx =>
        {
            speed = 2.4f;
        };//检测按住
        inputControl.Gameplay.WalkButton.canceled += ctx =>
        {
            speed = 5f;
        };//检测松开
        #endregion


        inputControl.Gameplay.Attack.started += OnAttackStarted;
        inputControl.Gameplay.Attack.canceled += OnAttackCanceled;



        inputControl.Gameplay.Pause.started += OnPause;

    }


    private void OnEnable()
    {
        inputControl.Enable();
    }
    private void OnDisable()
    {
        inputControl.Disable();
    }
    public void EnableGameplayInput()
    {
        inputControl.Gameplay.Enable();
    }//UIManager暂停菜单关闭调用
    public void DisableGameplayInput()
    {
        inputControl.Gameplay.Disable();
    }//UIManager暂停菜单打开调用







    #region  攻击触发
    [Header("攻击触发")]
    public float nextAttack = 0;//攻击冷却
    public float attackRate;//攻击频率
    private float attackPressTime;
    private float chargeThreshold = 0.35f;
    public GameObject attack_Collider_1;
    public GameObject attack_Collider_2;

    private void OnAttackStarted(InputAction.CallbackContext ctx)
    {
        attackPressTime = Time.time;
    }

    private void OnAttackCanceled(InputAction.CallbackContext ctx)
    {
        float holdTime = Time.time - attackPressTime;

        if (holdTime >= chargeThreshold)
        {
            ChargeAttack();//蓄力攻击
        }
        else
        {
            PlayerAttack(ctx);//单按一下
        }
    }
    public void ChargeAttack()
    {
        if (isDead) return;
        if (isHurt) return;
        if (isAttack) return;
        if (isDashAttack) return;

        if (Time.time > nextAttack)
        {


            nextAttack = Time.time + attackRate;
        }
    }
    void PlayerAttack(InputAction.CallbackContext obj)
    {

        if (isDead) return;
        if (isHurt) return;
        if (isAttack) return;
        if (isDashAttack) return;

        if (inputDirection.sqrMagnitude > 0.01f)
        {
            if (Time.time <= nextAttack || isAttack) return;//目前冲刺攻击给与attackRate的冷却


            //当这些动画在播放的时候
            AnimatorStateInfo state = playerAnimation.anim.GetCurrentAnimatorStateInfo(0);
            if (state.IsName("attack_1") ||state.IsName("attack_2") ||state.IsName("attack_3") ||state.IsName("attack_4"))
            {
                return;
            }



            //冲刺攻击
            StartCoroutine(DashAttack());
            nextAttack = Time.time + attackRate;
        }
        else
        {
            //这里是普通四连击,冲刺期间不能普攻
            if (isDashAttack) { return; }

            playerAnimation.PlayAttack();
            isAttack = true;
        }



    }


    private IEnumerator DashAttack()
    {
       



        isAttack = true;
        isDashAttack = true;

        dashDir = lastMoveDir.normalized;

        if (dashDir == Vector2.zero)
        {
            dashDir = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        }

        if (dashDir.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (dashDir.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);


        float timer = 0f;

        while (timer < dashAttackMoveTime)
        {
            if (CheckDashHitMap())
            {
                break;
            }

            if (isHurt || isDead) break;//滑行期间挂掉不移动

            rb.velocity = dashDir * dashAttackSpeed;

            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        rb.velocity = Vector2.zero;
        isDashAttack = false;
        //isAttack = false;
        // isAttack 继续交给动画结尾帧事件关闭
    }//冲刺攻击

    private bool CheckDashHitMap()
    {
        float checkDistance = 0.25f;

        RaycastHit2D hit = Physics2D.Raycast(
            rb.position,
            dashDir,
            checkDistance,
            mapLayer
        );



       

        return hit.collider != null;
    }

    #endregion




    private void OnPause(InputAction.CallbackContext ctx)
    {
        UIManager.instance.TogglePause();
    }
    #endregion
}
