using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;
using static GrabbableObject;
using static UnityEditor.Experimental.GraphView.GraphView;



public class Player : MonoBehaviour
{

    [Header("主动触发声音")]
    public FrameEvents frameEvents;

    private void Start()
    {
        RegisterHandle();//登录手柄控制

        UpdateAllBar();//更新UI


    }


    private void FixedUpdate()
    {
        if (isInputBlocked)
        {

            return;
        }


        if (isRape)
        {
            CheckAttack();

            ChangeStruggle(-1);

            rbody.simulated = false;
            ChangeSex(2);
            return;
        }//被捕获切断所有输入

        if (currentHealth <= 0) 
        {
            anim.Play("dead");
            rbody.simulated = false;
            return;
        }//死亡完全切断所有输入

        if (!isDie && currentHealth > 0)
        {

            BaseMove();//站走跑攻


        }
        else
        {
            rbody.velocity = Vector2.zero; // 停止所有移动
        }


        UpdateShadow();//控制影子大小

        // 每帧更新剑物体的旋转
        Strike_Effect.transform.Rotate(0, 0, 100 * Time.deltaTime);



        //当这些动画在播放的时候玩家不能移动
        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);


        if (state.IsName("attack_1") ||
            state.IsName("attack_2") ||
            state.IsName("attack_3") ||
            state.IsName("attack_4") ||
            state.IsName("rage") ||
            state.IsName("grab_throw") ||
            state.IsName("block") ||

            state.IsName("hurt_1") ||
            state.IsName("hurt_2") ||
            state.IsName("fly") ||

           state.IsName("Down") ||
           state.IsName("down") ||
           state.IsName("down_getup") ||
           state.IsName("dead")
           )
        {
            canMove = false;
        }
        else
        {
            canMove = true;
        }



        //轻击
        if (state.IsName("attack_1") ||
          state.IsName("attack_2") ||
          state.IsName("attack_3") ||
          state.IsName("attack_4"))
        {
            strike.TypeOfAttack = 0;
        }

        //重击
        if (state.IsName("run_attack") ||
         state.IsName("jump_attack"))
        {
            strike.TypeOfAttack = 1;
        }

    }

    public bool isRape = false;
    public bool isDie = false;

    /// <summary>
    /// 基础数值
    /// </summary>
    #region
    [Header("基础数值")]
    public Animator anim;//接入Spine动画机
    private float inputX, inputY;
    private float StopX, StopY;

    int moveSpeed = 0;//改动画器用的

    public Rigidbody2D rbody;//声明刚体
    float speed = 2; // 基础移动速度 （站0 走2 跑4）

    [Header("左侧目标和右侧目标")]
    public GameObject Target_Left;
    public GameObject Target_Right;

    private void BaseMove()
    {

        CheckAttack();
        CheckDodge();
        CheckJump();//不是按钮按下多久

        if (isKnockback) { return; }//被击飞时切断方向键输入

        //这个是拉杆控制，最优先，如果手柄没有输入，再检测手柄键盘等
        inputX = Joystick.Horizontal;
        inputY = Joystick.Vertical;
        Vector2 input = (transform.right * inputX + transform.up * inputY).normalized;//旋转摄像头

        if (inputX == 0 && inputY == 0)
        {

            input = moveAction.action.ReadValue<Vector2>();
            //Debug.Log("移动方向: " + input);

            // 记录原始输入值（四向判断用）
            inputX = input.x;
            inputY = input.y;

        }


        if (inputX > 0.5f)
        {
            inputX = 1; inputY = 0;
            attack.transform.rotation = Quaternion.Euler(0, 0, -90); // 右

        }
        else if (inputX < -0.5f)
        {
            inputX = -1; inputY = 0;
            attack.transform.rotation = Quaternion.Euler(0, 0, 90); // 左

        }
        else if (inputY > 0.5f && inputX > -0.5f && inputX < 0.5f)
        {
            inputX = 0; inputY = 1;
            attack.transform.rotation = Quaternion.Euler(0, 0, 0); // 上

        }
        else if (inputY < -0.5f && inputX > -0.5f && inputX < 0.5f)
        {
            inputX = 0; inputY = -1;
            attack.transform.rotation = Quaternion.Euler(0, 0, 180); // 下

        }
        //else { inputX = 0; inputY = 0; } // 静止时也归零

        // 保存上一次方向（用于静止状态播放对应Idle动画）
        if (inputX != 0 || inputY != 0)
        {
            StopX = inputX;
            StopY = inputY;
            if (isRunning)
            {
                moveSpeed = 2; speed = 4;



            }
            else
            {
                moveSpeed = 1; speed = 2;


            }


        }
        else
        {
            moveSpeed = 0;


        }

        if (inputY > -0.5f && inputY < 0.5f && inputX > -0.5f && inputX < 0.5f) { speed = 0; }//防止微微拉动拉杆也移动



       



        if (!canMove||isKnockback)
        {
            input = Vector2.zero;

        }//玩家只有在不攻击的时候才能移动，闪避的时候也无法叠加,被击飞时也无法移动



        rbody.velocity = input * speed;

        // 传给 Spine 动画机
        anim.SetFloat("InputX", StopX);
        //anim.SetFloat("InputY", StopY);

        anim.SetInteger("Speed", moveSpeed);

        // 可以加一个简易翻面处理（仅左右）
        if (StopX < 0)
            anim.gameObject.transform.localScale = new Vector3(-1, 1, 1);
        else if (StopX > 0)
            anim.gameObject.transform.localScale = new Vector3(1, 1, 1);
    }


    #endregion




    /// <summary>
    /// 攻击系统
    /// </summary>
    #region
    [Header("蓄力攻击")]
    private float attackPressTime = 0f;      // 持续按下时长计时器
    private bool attackTriggered = false;    // 是否已经触发攻击动作（防止反复触发）

    public bool canMove = true;

    public GameObject attack;//伤害朝向
    public GameObject attack_Collider;//伤害碰撞体

    public Strike strike;

    void Attack_Start()
    {
        isAttacking = true;
        attackPressTime = 0f;

        attackTriggered = false;
    }

    void Attack_Cancel()
    {
        isAttacking = false;




        if (!attackTriggered)
        {


            if (attackPressTime < 0.2f)
            {


                if (isHoldingObject) 
                {
                    anim.SetTrigger("Throw");

                    return;

                }//如果手上持有物品，那么优先扔出去            

                AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
                if (state.IsName("jump_attack"))
                {
                    return;
                }//防止连续飞踢

                if (!IsGrounded())
                {
                    //这个落地依旧触发down，有没有方法将被踢飞的isGround和踢击isGround区别开来
                    if (StopX < 0)
                        FlyKick(-3);
                    else if (StopX > 0)
                        FlyKick(3);
                    anim.Play("jump_attack");
                }
                else if (isRunning)
                {
                    PlayDodge(); // 闪避
                    anim.Play("run_attack");
                }
                else
                {
                    PlayNormalAttack(); // 普通攻击
                }

            }
            else
            {
                PlayChargeAttack(); // 蓄力攻击
            }

            attackPressTime = 0;

            attackTriggered = true;


        }




    }

    void CheckAttack()
    {
        if (isAttacking && !attackTriggered)
        {
            attackPressTime += Time.deltaTime;

            if (attackPressTime >= 0.2f)
            {

            }

            //ChangeCritical(10);//按下暴击率快速上升
        }
        else
        {
            //ChangeCritical(-5);//松开暴击率快速下降
        }
    }

    [Header("攻击")]
    public int currentCombo = 0;
    public bool isAttacking2 = false;
    public bool canCombo = false;
    public bool comboQueued = false;


    public void PlayNormalAttack()
    {

        TryCrit();

        if (!isAttacking2)
        {
            StartCombo();
        }
        else if (canCombo)
        {
            comboQueued = true;
        }

    }



    void StartCombo()
    {
        currentCombo = 1;
        isAttacking2 = true;
        anim.Play("attack_1", 0, 0);

       
    }

    public void ResetCombo()
    {
        currentCombo = 0;
        comboQueued = false;
        canCombo = false;
        isAttacking2 = false;
        anim.Play("stand");


    }






    public void PlayChargeAttack()
    {
        TryCrit(); // 改用新方法触发暴击
        strike.chargeTime = attackPressTime; // 把蓄力时间传过去（蓄力那段时间也能成攻击力 能加上去）

        anim.Play("rage");

    }//蓄力攻击


    public void AttackVoice()
    {
        switch (Random.Range(0, 3))
        {
            case 0:
                frameEvents._Attack_sword_chop1();
                break;
            case 1:
                frameEvents._Attack_sword_chop2();
                break;
            case 2:
                frameEvents._Attack_sword_chop3();
                break;
        }
    }//攻击声音




    #endregion


    /// <summary>
    /// 闪避(冲刺)与跳跃系统
    /// </summary>
    #region
    [Header("闪避（跳跃）键按下")]
    private float dodgePressTime = 0f;      // 持续按下时长计时器
    private bool dodgeTriggered = false;    // 是否已经触发攻击动作（防止反复触发）




    void Dodge_Start()
    {
        if (!isDie)
        {
            isDodging = true;
            dodgePressTime = 0f;

            dodgeTriggered = false;
        }

    }
    void Dodge_Cancel()
    {

        if (!isDie)
        {
            isDodging = false;

            if (!dodgeTriggered)
            {
                if (dodgePressTime < 0.2f)
                {
                    //PlayDodge(); // 闪避
                    PlayJump();
                }
                else
                {
                    //魔族变身

                    //PlayChargeAttack(); // 蓄力攻击
                }

                dodgePressTime = 0;

                dodgeTriggered = true;
            }
        }



    }

    void CheckDodge()
    {
        if (isDodging && !dodgeTriggered)
        {
            dodgePressTime += Time.deltaTime;

        }
    }





    [Header("模拟跳跃")]
    // 模拟跳跃高度
    public float zHeight = 0f; // 相对当前Platform（地面基准）的离地高度
    float zVelocity = 0f;
    float gravity = -20f; // 可以调成 -20f 更快落下
    float jumpForce = 10f;//原来是5f

    [Header("地面基准与公差")]
    public float GroundEpsilon = 0.02f; // 落地公差：越大越“黏地”

    // 角色跳跃偏移对象（Spine动画对象）
    public float groundY = 0f; // 初始化地面位置// 世界坐标中的跑道基线Y（不含平台厚度）

    bool wasInAir = false; // 前一帧是否在空中


    // 将 Platform 作为“当前地面基准Z”（例如 0 / 3 / 1.5 ...）
    // 用属性保证切换平台时不会“瞬移”
    [SerializeField] private float _platform = 0.01f;
    public float Platform
    {
        get => _platform;
        set
        {
            if (Mathf.Approximately(_platform, value)) return;

            // 切换平台：保持“世界高度（_platform + zHeight）”不变
            float worldZBefore = _platform + zHeight;
            _platform = value;
            zHeight = worldZBefore - _platform;

            // 若当前已在地面附近，直接夹到0，避免负值渗入
            if (zHeight < 0f && Mathf.Abs(zHeight) <= GroundEpsilon)
                zHeight = 0f;
        }
    }

    public void PlayJump()
    {
        if (IsGrounded())
        {

            // 让平台暂时“不可判定”，允许跳起来
            //zHeight = Platform + 0.01f; // 立刻把高度略微抬高，脱离地面

            //Debug.Log("跳跃");
            zVelocity = jumpForce;
            airborneState = AirborneType.Jump;////////////////////////////////////////////////////////////////(跳起和飞踢落地都需要这个)
            //frameEvents._SE_Clothes();
        }


    }


    IEnumerator TriggerKnockdownOnLanding()
    {
        yield return null; // 等一帧，防止被 BaseMove 覆盖

        Knockdown();
    }//冻结一切输入立刻触发倒地动画

    [Header("当玩家在空中的时候可以隐藏碰撞体防止卡一下")]
    public CircleCollider2D Collider2D;


    void CheckJump()
    {
        // 1) 重力与高度积分（相对当前Platform的高度）
        zVelocity += gravity * Time.deltaTime;
        zHeight += zVelocity * Time.deltaTime;



        //bool isGroundedNow = zHeight <= 0f;
        // 2) 是否落地（只看相对高度 zHeight）
        bool isGroundedNow = zHeight <= GroundEpsilon;

        if (isGroundedNow)
        {


            if (wasInAir)
            {
                //frameEvents._Effect_falldown(); // 公共落地音效

                if (airborneState == AirborneType.Jump)
                {
                    // 普通跳跃落地
                }
                else if (airborneState == AirborneType.Knocked)
                {
                    // 被踢击落地
                    // Knockdown();
                    //
                    // Debug.Log(" 被踢击落地");
                    // anim.SetTrigger("Player_Down");//因为被击落地动画器怎么也转不过来所以只能

                    // ✔ 立刻冻结
                    inputX = 0;
                    inputY = 0;
                    StopX = 0;
                    StopY = 0;
                    rbody.velocity = Vector2.zero;
                    moveSpeed = 0;

                    canMove = false;

                    // ✔ 直接让动画跳进 down，不交给状态机判断
                    anim.Play("down", 0, 0);

                    // ✔ 设定标记，在下一帧正式 Knockdown()
                    StartCoroutine(TriggerKnockdownOnLanding());

                    //Debug.Log("【击飞落地】冻结输入 + 播放 down");
                }
            }



            // 3) 归零相对高度，归零垂直速度
            zHeight = 0f;
            zVelocity = 0f;
            knockbackX = 0f;

          
            // 4) 重新计算 groundY：把“当前世界Y - Platform”作为基线
            //   这样下一帧用统一公式回写位置不会跳变
            groundY = transform.position.y - Platform;



            airborneState = AirborneType.None; // 重置状态
           
        }


        // 5) 统一位置回写
        Vector3 pos = transform.position;
        if (!isGroundedNow) // 空中
        {
            pos.y = groundY + Platform + zHeight;
            pos.z = -1f;
            anim.SetBool("Jump", true);
            if (Collider2D) Collider2D.enabled = false;
        }
        else // 落地
        {
            pos.y = groundY + Platform;
            pos.z = 0f;
            anim.SetBool("Jump", false);
            if (Collider2D) Collider2D.enabled = true;
        }
        transform.position = pos;






        //if (zHeight > Platform)
        //{
        //    Vector3 pos = transform.position;
        //    pos.y = groundY + zHeight;
        //    pos.z = -1f; // 跳跃时到前面
        //    transform.position = pos;
        //
        //    anim.SetBool("Jump", true);
        //
        //
        //    //当玩家在空中的时候可以隐藏碰撞体防止卡一下
        //    Collider2D.enabled = false;
        //
        //
        //}
        //else
        //{
        //    Vector3 pos = transform.position;
        //
        //    pos.y = groundY; // 一定不要用 Platform
        //
        //    pos.z = 0f; // 落地恢复排序
        //    transform.position = pos;
        //    anim.SetBool("Jump", false);
        //
        //
        //    //落地出现碰撞体
        //    Collider2D.enabled = true;
        //}




        // 6) 记录上一帧是否在空中
        wasInAir = !isGroundedNow;



        // 7) 水平击飞（仍按你原逻辑）
        if (!IsGrounded() && knockbackX != 0f)
        {
            transform.position += new Vector3(knockbackX * Time.deltaTime, 0f, 0f);
        }

        if (isGroundedNow)
        {
            knockbackX = 0f; // 落地停止水平击飞
        }
    }


    public bool IsGrounded()
    {
        //return zHeight <= 0.01f; // 只要高度为 0 即为落地
        //return zHeight <= Platform; //通过改变这个来让玩家被定在空中

        // 只看相对高度是否接近0，而不是和 Platform 比较
        return zHeight <= GroundEpsilon;
    }
    //public float Platform = 0.01f;


    [Header("影子控制")]
    public Transform shadow;              // 影子对象
    public Vector2 shadowBaseScale = new Vector2(1f, 1f); // 原始大小
    public float shadowMinScale = 0.3f;   // 最小缩放（跳最高时）
    public float maxJumpHeight = 3f;      // 最大跳跃高度（用于缩放比例）

    void UpdateShadow()
    {
        if (shadow == null) return;

        // 1. 保持影子在地面（角色 X，地面 Y）
        Vector3 pos = transform.position;
        shadow.position = new Vector3(pos.x, groundY + Platform, pos.z);

        // 2. 计算当前缩放（高度越高越小）
        float t = Mathf.Clamp01(zHeight / maxJumpHeight); // 0~1
        float scale = Mathf.Lerp(1f, shadowMinScale, t);  // 1 ~ 最小
        shadow.localScale = shadowBaseScale * scale;

        // 可选：你也可以改变 Alpha 值
        //var color = shadow.GetComponent<SpriteRenderer>().color;
        //color.a = Mathf.Lerp(1f, 0.6f, t);
        //shadow.GetComponent<SpriteRenderer>().color = color;

        var sr = shadow.GetComponent<SpriteRenderer>();
        if (sr)
        {
            var color = sr.color;
            color.a = Mathf.Lerp(1f, 0.6f, t);
            sr.color = color;
        }
    }

    [Header("被击飞与飞踢")]
    bool isKnockback = false;
    enum AirborneType { None, Jump, Knocked }
    AirborneType airborneState = AirborneType.None;
    float knockbackX = 0f;


    public void Knockback(float force)
    {
        knockbackX = force;
        zVelocity = 10f;
        airborneState = AirborneType.Knocked;
        isKnockback = true;
        anim.gameObject.transform.localScale = new Vector3(force < 0 ? -1 : 1, 1, 1);
    }

    public void FlyKick(float force)
    {
        knockbackX = force;
        zVelocity = 2f;
        airborneState = AirborneType.Jump;
        anim.gameObject.transform.localScale = new Vector3(force < 0 ? -1 : 1, 1, 1);
    }


    //float knockbackX = 0f; // 击飞时的水平速度（正负代表方向）
    //
    //public void Knockback(float force)
    //{
    //    knockbackX = force;
    //    zVelocity = 10;//这里被击飞力度
    //    airborneState = AirborneType.Knocked; // 设置为不可操作
    //    isKnockback = true;//被击飞期间无法受伤/切断输入
    //
    //    if (knockbackX < 0)
    //        anim.gameObject.transform.localScale = new Vector3(-1, 1, 1);
    //    else if (knockbackX > 0)
    //        anim.gameObject.transform.localScale = new Vector3(1, 1, 1);
    //}
    //
    //public void FlyKick(float force)
    //{
    //    knockbackX = force;
    //    zVelocity = 2;//飞踢力度
    //    airborneState = AirborneType.Jump; // 可操作
    //
    //    if (knockbackX < 0)
    //        anim.gameObject.transform.localScale = new Vector3(-1, 1, 1);
    //    else if (knockbackX > 0)
    //        anim.gameObject.transform.localScale = new Vector3(1, 1, 1);
    //}
    //
    ////分开落地触发
    //enum AirborneType { None, Jump, Knocked }
    //
    //AirborneType airborneState = AirborneType.None;
    //
    //private bool isKnockback = false; // 被击飞中无法输入、无法受击

    [Header("闪避触发")]


    public float dodgeSpeed = 10f;
    public float dodgeDistance = 0.5f;
    public LayerMask obstacleLayer;

    public bool isDodge = false;//闪避动画期间的Dodge


    void PlayDodge()
    {
        if (!isOutOfStrength)
        {
            if (isDodge) return;//防止连续闪避


            Vector2 dodgeDir = new Vector2(StopX, StopY).normalized;//站立的时候向后闪避
            if (dodgeDir == Vector2.zero) return;

            StartCoroutine(Dodge(dodgeDir, 15f, 2f));


            //手动再添加一个冷却
            isOutOfStrength = true;
            Invoke("OutOfStrengthCollDown", 0.5f);
        }

    }

    [Header("冷却提示")]
    bool isOutOfStrength = false;
    void OutOfStrengthCollDown()
    {
        isOutOfStrength = false;
    }


    IEnumerator Dodge(Vector2 direction, float dodgeSpeed, float dodgeDistance)
    {

        //闪避后连击取消
        if (currentHealth > 0)
        {
            Invoke("ResetCombo", 1f);//防止挂了又站起来
        }


        // 音效、体力扣除
        //frameEvents._SE_Clothes();






        isDodge = true;

        canMove = false; // 【在闪避的一段时间内无法上下左右移动】防止位移冲突

        float movedDistance = 0f;


        while (movedDistance < dodgeDistance)
        {
            float step = dodgeSpeed * Time.fixedDeltaTime;

            Vector3 newPos = rbody.position + direction * step;

            // 检测闪避方向是否有障碍物（使用 BoxCast 替代 Raycast）
            Vector2 boxSize = new Vector2(0.5f, 0.5f); // 角色体积大小，请根据实际角色尺寸设置
            if (Physics2D.BoxCast(rbody.position, boxSize, 0f, direction, 0.1f, obstacleLayer))
            {
                Debug.Log("障碍物检测到，终止闪避");
                break;
            }

            rbody.MovePosition(newPos);  // 物理安全移动
            movedDistance += step;

            yield return new WaitForFixedUpdate();
        }



        Invoke(nameof(DodgingOver), 0.6f);// 让子弹时间更容易触发

        canMove = true; // 【在闪避的一段时间内无法上下左右移动】防止位移冲突




    }

    void DodgingOver()
    {
        isDodge = false;
    }


    #endregion


    /// <summary>
    /// 投掷系统
    /// </summary>
    #region
    [Header("投掷物品")]
    public bool isHoldingObject = false;//是否抓住物品
    public GameObject Obstacle_Attack;

    GrabbableObject.GrabbableType heldItemType;

    public CharacterSkin characterSkin;//角色
    public void OnGrabCollision(GrabbableObject.GrabbableType item)
    {
        if (!isHoldingObject)
        {
            isHoldingObject = true;//当玩家举起物品的时候无法跑步，无法跳跃


            anim.SetBool("IsGrabbing", true);

            heldItemType = (GrabbableType)item; // 进行 enum 转换
            characterSkin.ShowCurrentAll(heldItemType);
        }
    }




    public void ThrowHeldObject() // 由 grab_throw 动画末尾事件触发
    {
        GameObject obj = Instantiate(Obstacle_Attack, transform.position, Quaternion.identity);

        // 方向判断（以角色朝向为基准）
        float dir = StopX > 0 ? 1f : -1f;

        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.simulated = true;
            rb.velocity = new Vector2(4f * dir, 2f); // 水平+上抛弧线，可调整【8/5】
        }

        // 激活爆炸逻辑
        ThrowHeldObject script = obj.GetComponent<ThrowHeldObject>();
        if (script != null)
        {
            script.Launch(heldItemType);
        }

        // 播完投掷动画后，角色状态复位
        isHoldingObject = false;
        anim.SetBool("IsGrabbing", false);
    }

    #endregion



    /// <summary>
    /// 多端输入
    /// </summary>
    #region
    [Header("InputSystem")]
    [SerializeField] private InputActionReference moveAction;//方向键控制
    [SerializeField] private InputActionAsset inputActions;//跑攻闪

    public bool isInputBlocked = false;//在暂停菜单界面暂时切断玩家的输入

    private InputAction runAction;

    private InputAction AttackAction;

    private InputAction DodgeAction;


    private void RegisterHandle()
    {
        // 获取动作（根据你的Action Map结构可能需要调整路径）
        runAction = inputActions.FindAction("Run");
        AttackAction = inputActions.FindAction("Attack");
        DodgeAction = inputActions.FindAction("Dodge");

        // 订阅输入事件
        runAction.started += OnRunStarted;
        runAction.canceled += OnRunCanceled;

        // 订阅输入事件
        AttackAction.started += OnAttackStarted;
        AttackAction.canceled += OnAttackCanceled;

        // 订阅输入事件
        DodgeAction.started += OnDodgeStarted;
        DodgeAction.canceled += OnDodgeCanceled;


    }

    
    private void OnDisable()
    {
        if (runAction != null)
        {
            runAction.started -= OnRunStarted;
            runAction.canceled -= OnRunCanceled;
        }

        if (AttackAction != null)
        {
            AttackAction.started -= OnAttackStarted;
            AttackAction.canceled -= OnAttackCanceled;
        }

        if (DodgeAction != null)
        {
            DodgeAction.started -= OnDodgeStarted;
            DodgeAction.canceled -= OnDodgeCanceled;
        }
    }//重刷场景时自动解绑

    private void OnEnable()
    {
        RegisterHandle();
    }//重刷场景时自动解绑



    private void OnRunStarted(InputAction.CallbackContext context)
    {
        if (!isDie && !isKnockback && currentHealth > 0 && !isInputBlocked)
        {
            if (!isHoldingObject) 
            {
                isRunning = true;
            }//抓住物品无法跑

           
        }
    }
    private void OnRunCanceled(InputAction.CallbackContext context)
    {
        if (!isDie && !isKnockback && currentHealth > 0 && !isInputBlocked)
        {
            if (!isHoldingObject)
            {
                isRunning = false;
            }//抓住物品无法跑
           
        }
    }

    private void OnAttackStarted(InputAction.CallbackContext context)
    {
        if (isRape) { Struggle_Start(); }
        else if (!isDie && !isKnockback && currentHealth > 0 && !isInputBlocked)
        {
            Attack_Start();
        }
    }
    private void OnAttackCanceled(InputAction.CallbackContext context)
    {
        if (isRape) { Struggle_Cancel(); }
        else if (!isDie && !isKnockback && currentHealth > 0 && !isInputBlocked)
        {
            Attack_Cancel();
        }
    }

    private void OnDodgeStarted(InputAction.CallbackContext context)
    {
        if (!isDie && !isKnockback && currentHealth > 0 && !isInputBlocked)
        {
            if (!isHoldingObject)
            {
                Dodge_Start();

            }//抓住物品无法跳
           
        }
    }
    private void OnDodgeCanceled(InputAction.CallbackContext context)
    {
        if (!isDie && !isKnockback && currentHealth > 0 && !isInputBlocked)
        {

            if (!isHoldingObject)
            {
                Dodge_Cancel();

            }//抓住物品无法跳
           
        }
    }

    [Header("手机端触发")]
    public Joystick Joystick;

    //手机端触发
    public bool isRunning = false;//持续按下跑步键
    public void ButtonSetRun()
    {
        if (!isDie && !isKnockback && currentHealth > 0 && !isInputBlocked)
        {
            isRunning = true;
        }

    }
    public void ButtonSetStop()
    {
        if (!isDie && !isKnockback && currentHealth > 0 && !isInputBlocked)
        {
            isRunning = false;
        }

    }

    //手机端触发
    public bool isAttacking = false;//持续按下攻击键
    public void ButtonSetAttack()
    {
        if (isRape) { Struggle_Start(); }
        else if (!isDie && !isKnockback && currentHealth > 0 && !isInputBlocked)
        {
            Attack_Start();
        }

    }
    public void ButtonSetAttackOver()
    {

        if (isRape) { Struggle_Cancel(); }
        else if (!isDie && !isKnockback && currentHealth > 0 && !isInputBlocked)
        {
            Attack_Cancel();
        }

    }

    //手机端触发
    public bool isDodging = false;//持续按下闪避键
    public void ButtonSetDodge()
    {

        if (!isDie && !isKnockback && currentHealth > 0 && !isInputBlocked)
        {
            Dodge_Start();
        }

    }
    public void ButtonSetDodgeOver()
    {
        if (!isDie && !isKnockback && currentHealth > 0 && !isInputBlocked)
        {
            Dodge_Cancel();
        }

    }

    #endregion




    /// <summary>
    /// 生命值体力值等数值
    /// </summary>
    #region

    void UpdateAllBar()
    {
        //更新UI
        UIManager.instance.UpdateSexBar(currentSex, maxSex);
        UIManager.instance.UpdateHealthBar(currentHealth, maxHealth);
        UIManager.instance.UpdateCriticalBar(currentCritical, maxCritical);
    }
    [Header("特效")]
    public GameObject Strike_Effect;//剑光特效
    public GameObject Hit_Effect;//打击特效
    public GameObject BloodEffect;//受伤特效
    public GameObject SparkEffect;//火星特效


    [Header("生命值")]
    public int currentHealth;
    public int maxHealth;


    [Header("伤害显示")]
    public GameObject RedScreen;
    public bool isScreaming;
    public HudText HudText;

    [Header("暴击")]
    public GameObject Critial;

    public void ChangeHealth(int amount, int TypeOfAttack)//【攻击方式】  0轻攻击(打击特效)  1重攻击（击飞）(打击特效)  2剑击特效 
    {
        if (!isScreaming&&currentHealth> 0 && !isDie && IsGrounded() && !isKnockback)//冷却不受击，死亡后不受击，倒地不受击，玩家在空中的时候不受击（跳跃/被击飞），被击飞不受击
        {




            if (amount < 0)
            {


                if (Random.Range(0, 4) == 0 && canMove&&!isHoldingObject) //处于攻击，举起物品状态下无法防御
                {
                    anim.Play("block");

                    switch (Random.Range(0, 3))
                    {
                        case 0:
                            frameEvents._Attack_sword_clash2();
                            break;
                        case 1:
                            frameEvents._Attack_sword_clash3();
                            break;
                        case 2:
                            frameEvents._Attack_sword_clash4();
                            break;
                    }


                    //显示伤害
                    HudText.HUD(0);//0会显示Miss

                    return;
                }


                if (Random.Range(0, 2) == 1)
                {
                    if (currentHealth > 0 && IsGrounded()) 
                    {
                        //击倒再站起(和暴击结合)只有站在地上才能被击倒
                        Critial.SetActive(true);
                        Knockdown();
                    }

                }
                else
                {
                    switch (Random.Range(0, 2))
                    {
                        case 0:
                            anim.Play("hurt_1");
                            break;
                        case 1:
                            anim.Play("hurt_2");
                            break;
                    }
                    Invoke("Fly", 0.3f);//如果在空中触发被击飞动画


                    // 可以加一个简易翻面处理（仅左右）
                    if (StopX < 0)
                        Knockback(3);
                    else if (StopX > 0)
                        Knockback(-3);
                }


                // if (isHoldingObject) 
                // {
                //     ThrowHeldObject();
                // }//举着物品被攻击时物品会扔出去



                //血特效
                GameObject effectPrefabs = Instantiate(BloodEffect, transform.position, transform.rotation);
                Destroy(effectPrefabs, 2f);

                RedScreen.SetActive(true);

            }//格挡

            //伤害类型
            switch (TypeOfAttack)
            {
                case 0:

                    break;

                case 1:
                    Hit_Effect.SetActive(true);//打击伤害
                    break;

                case 2:
                    Strike_Effect.SetActive(true);//剑伤害
                    break;
                case 3:
                    //Palsy_Effect.SetActive(true);//雷电伤害
                    break;
            }



            currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
            UIManager.instance.UpdateHealthBar(currentHealth, maxHealth);

            //显示伤害
            HudText.HUD(amount);

            //有1秒左右的伤害冷却
            Invoke("HurtOver", 0.5f);
         
            isScreaming = true;



            switch (Random.Range(0, 3))
            {
                case 0:
                    frameEvents._Attack_blood1();
                    break;
                case 1:
                    frameEvents._Attack_blood2();
                    break;
                case 2:
                    frameEvents._Attack_blood3();
                    break;
            }

          






            if (currentHealth <= 0)
            {
                isDie = true;


                anim.Play("dead");

                return;
            }
            else
            {
                //死亡时收到伤害不再重置站姿
                //受伤时连击取消
                Invoke("ResetCombo", 1f);

            }



        }

    }
    void Fly()
    {
        if (!IsGrounded())
        {
            anim.Play("fly");
        }

    }
    public void Knockdown()
    {

        isDie = true;

        //anim.CrossFade("down", 0f);//强制播放
        anim.Play("down");

        //防止最后一下又击倒站起
        if (currentHealth > 0)
        {
            Invoke("GetUp", 0.5f);//比起敌人，玩家可以更快站起来
        }


        //目前玩家每次落地都触发，改为被暴击触发
        //Critial.SetActive(true);

    }//击倒

    void HurtOver()
    {
        isScreaming = false;
        RedScreen.SetActive(false);
    }//有1秒左右的伤害冷却



    void GetUp()
    {
        isDie = false;
        anim.Play("down_getup");

        isKnockback = false; // ✔ 落地结束击飞状态
    }




    [Header("淫乱值")]
    public int currentSex;
    public int maxSex;



    public void ChangeSex(int amount)
    {

        currentSex = Mathf.Clamp(currentSex + amount, 0, maxSex);
        UIManager.instance.UpdateSexBar(currentSex, maxSex);
    }


    [Header("UI条 暴击值")]

    public int currentCritical;
    public int maxCritical;


    public void ChangeCritical(int amount)
    {

        //Debug.Log("充能");
        if (!isDie)
        {


        }//如果是已经Die了，那么这个

        currentCritical = Mathf.Clamp(currentCritical + amount, 0, maxCritical);
        UIManager.instance.UpdateCriticalBar(currentCritical, maxCritical);
    }

    private void TryCrit()
    {

        // 计算当前暴击率
        float critRate = (float)currentCritical / (float)maxCritical;

        // 只有在暴击率大于等于 60% 时，才可能暴击
        if (critRate >= 0.6f)
        {
            strike.isCritial = true;


        }
        else
        {
            strike.isCritial = false;
        }
    }

    [Header("挣扎值")]
    public int currentStruggle;
    public int maxStruggle;

    public void ChangeStruggle(int amount)
    {

        currentStruggle = Mathf.Clamp(currentStruggle + amount, 0, maxStruggle);
        UIManager.instance.UpdateStruggleBar(currentStruggle, maxStruggle);

        if (currentStruggle >= maxStruggle) 
        {
            Debug.Log("挣扎成功");
            EscapeFromRape();

        }
    }

    bool struggleTriggered = false;
    void Struggle_Start() 
    {
        struggleTriggered = false;
    }
    void Struggle_Cancel()
    {

        if (!struggleTriggered) 
        {
            ChangeStruggle(100);
            struggleTriggered = true;
        }
    }

    public GameObject enemyRaper;
    public List<Enemy> observingEnemies = new List<Enemy>();//围观的敌人
    public void EscapeFromRape()
    {
        // 恢复影子可见
        shadow.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 143f / 255f); // α = 143/255

        // 恢复身体显示
        characterSkin.ShowSkeleton();

        // 玩家重置状态
        isRape = false;
        isDie = false; // 如果你希望挣扎成功后恢复行动

        // 播放起身动画（可选）
        anim.Play("down_getup");

        // 找到正在抓住的敌人，也让他恢复
        if (enemyRaper != null)
        {
            enemyRaper.GetComponent<Enemy>().ReleasePlayer();
            enemyRaper = null;
        }

        ChangeStruggle(-maxStruggle);
        rbody.simulated = true;

        // 清空所有围观敌人状态
        foreach (Enemy e in observingEnemies)
        {
            if (e != null)
            {
                e.isRape = false;
            }
        }
        observingEnemies.Clear();
    }
    #endregion

}
