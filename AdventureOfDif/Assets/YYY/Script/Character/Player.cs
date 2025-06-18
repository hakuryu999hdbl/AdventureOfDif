using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;



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

        if (currentHealth <= 0) 
        {
            anim.Play("dead");
            rbody.simulated = false;
            return;
        }

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
            state.IsName("hurt_1") ||
            state.IsName("hurt_2") ||

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



    }

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



        CheckAttack();
        CheckDodge();
        CheckJump();



        if (!canMove)
        {
            input = Vector2.zero;

        }//玩家只有在不攻击的时候才能移动，闪避的时候也无法叠加



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

            ChangeCritical(10);//按下暴击率快速上升
        }
        else
        {
            ChangeCritical(-5);//松开暴击率快速下降
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
    float zHeight = 0f;
    float zVelocity = 0f;
    float gravity = -20f; // 可以调成 -20f 更快落下
    float jumpForce = 10f;//原来是5f

    // 角色跳跃偏移对象（Spine动画对象）
    float groundY = 0f; // 初始化地面位置
    bool wasInAir = false; // 前一帧是否在空中
    public void PlayJump()
    {
        if (IsGrounded())
        {
            //Debug.Log("跳跃");
            zVelocity = jumpForce;
            airborneState = AirborneType.Jump;////////////////////////////////////////////////////////////////(跳起和飞踢落地都需要这个)
            frameEvents._SE_Clothes();
        }


    }

    void CheckJump()
    {
        // 应用重力
        zVelocity += gravity * Time.deltaTime;
        zHeight += zVelocity * Time.deltaTime;
        //if (!isDodge) 
        //{
        //    zHeight += zVelocity * Time.deltaTime;
        //}


        bool isGroundedNow = zHeight <= 0f;

        if (isGroundedNow)
        {
            if (wasInAir)
            {
                frameEvents._Effect_falldown(); // 公共落地音效

                if (airborneState == AirborneType.Jump)
                {
                    // 普通跳跃落地
                }
                else if (airborneState == AirborneType.Knocked)
                {
                    // 被踢击落地
                    Knockdown();

                    Debug.Log(" 被踢击落地");
                    anim.SetTrigger("Player_Down");//因为被击落地动画器怎么也转不过来所以只能
                }
            }

            zHeight = 0f;
            zVelocity = 0f;
            knockbackX = 0f;
            groundY = transform.position.y;
            airborneState = AirborneType.None; // 重置状态
            isKnockback = false; // ✔ 落地结束击飞状态
        }

        if (zHeight > 0f)
        {
            Vector3 pos = transform.position;
            pos.y = groundY + zHeight;
            transform.position = pos;

            anim.SetBool("Jump", true);
        }
        else
        {
            anim.SetBool("Jump", false);
        }

        // 更新前一帧状态
        wasInAir = !isGroundedNow;


       

        //被击飞
        if (!IsGrounded() && knockbackX != 0f)
        {
            transform.position += new Vector3(knockbackX * Time.deltaTime, 0f, 0f);
        }

        if (isGroundedNow)
        {
            knockbackX = 0f; // 落地停止水平击飞
        }
    }

    private bool IsGrounded()
    {
        return zHeight <= 0.01f; // 只要高度为 0 即为落地
    }



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
        shadow.position = new Vector3(pos.x, groundY, pos.z);

        // 2. 计算当前缩放（高度越高越小）
        float t = Mathf.Clamp01(zHeight / maxJumpHeight); // 0~1
        float scale = Mathf.Lerp(1f, shadowMinScale, t);  // 1 ~ 最小
        shadow.localScale = shadowBaseScale * scale;

        // 可选：你也可以改变 Alpha 值
        var color = shadow.GetComponent<SpriteRenderer>().color;
        color.a = Mathf.Lerp(1f, 0.6f, t);
        shadow.GetComponent<SpriteRenderer>().color = color;
    }

    [Header("被击飞与飞踢")]
    float knockbackX = 0f; // 击飞时的水平速度（正负代表方向）

    public void Knockback(float force)
    {
        knockbackX = force;
        zVelocity = 10;//这里被击飞力度
        airborneState = AirborneType.Knocked; // 设置为不可操作
        isKnockback = true;//被击飞期间无法受伤/切断输入

        if (knockbackX < 0)
            anim.gameObject.transform.localScale = new Vector3(-1, 1, 1);
        else if (knockbackX > 0)
            anim.gameObject.transform.localScale = new Vector3(1, 1, 1);
    }

    public void FlyKick(float force)
    {
        knockbackX = force;
        zVelocity = 2;//飞踢力度
        airborneState = AirborneType.Jump; // 可操作

        if (knockbackX < 0)
            anim.gameObject.transform.localScale = new Vector3(-1, 1, 1);
        else if (knockbackX > 0)
            anim.gameObject.transform.localScale = new Vector3(1, 1, 1);
    }

    //分开落地触发
    enum AirborneType { None, Jump, Knocked }

    AirborneType airborneState = AirborneType.None;

    private bool isKnockback = false; // 被击飞中无法输入、无法受击

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
        frameEvents._SE_Clothes();






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
    /// 多端输入
    /// </summary>
    #region
    [Header("InputSystem")]
    [SerializeField] private InputActionReference moveAction;//方向键控制
    [SerializeField] private InputActionAsset inputActions;//跑攻闪

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
    private void OnRunStarted(InputAction.CallbackContext context)
    {
        if (!isDie && !isKnockback && currentHealth > 0)
        {
            isRunning = true;
        }
    }
    private void OnRunCanceled(InputAction.CallbackContext context)
    {
        if (!isDie && !isKnockback && currentHealth > 0)
        {
            isRunning = false;
        }
    }

    private void OnAttackStarted(InputAction.CallbackContext context)
    {
        if (!isDie && !isKnockback && currentHealth > 0)
        {
            Attack_Start();
        }
    }
    private void OnAttackCanceled(InputAction.CallbackContext context)
    {
        if (!isDie && !isKnockback && currentHealth > 0)
        {
            Attack_Cancel();
        }
    }

    private void OnDodgeStarted(InputAction.CallbackContext context)
    {
        if (!isDie && !isKnockback && currentHealth > 0)
        {
            Dodge_Start();
        }
    }
    private void OnDodgeCanceled(InputAction.CallbackContext context)
    {
        if (!isDie && !isKnockback && currentHealth > 0)
        {
            Dodge_Cancel();
        }
    }

    [Header("手机端触发")]
    public Joystick Joystick;

    //手机端触发
    public bool isRunning = false;//持续按下跑步键
    public void ButtonSetRun()
    {
        if (!isDie && !isKnockback && currentHealth > 0)
        {
            isRunning = true;
        }

    }
    public void ButtonSetStop()
    {
        if (!isDie && !isKnockback && currentHealth > 0)
        {
            isRunning = false;
        }

    }

    //手机端触发
    public bool isAttacking = false;//持续按下攻击键
    public void ButtonSetAttack()
    {
        if (!isDie && !isKnockback && currentHealth > 0)
        {
            Attack_Start();
        }

    }
    public void ButtonSetAttackOver()
    {
        if (!isDie && !isKnockback && currentHealth > 0)
        {
            Attack_Cancel();
        }

    }

    //手机端触发
    public bool isDodging = false;//持续按下闪避键
    public void ButtonSetDodge()
    {

        if (!isDie && !isKnockback && currentHealth > 0)
        {
            Dodge_Start();
        }

    }
    public void ButtonSetDodgeOver()
    {
        if (!isDie && !isKnockback && currentHealth >0)
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
    }
    [Header("特效")]
    public GameObject Strike_Effect;//剑光特效
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

    public void ChangeHealth(int amount, int TypeOfAttack)//【攻击方式】 0无  1剑击特效  2闪电特效  3冻结
    {
        if (!isScreaming&&currentHealth>0 &&IsGrounded())//死亡后不受击，玩家在空中的时候不受击
        {




            if (amount < 0)
            {



                //击倒再站起(和暴击结合)只有站在地上，不处于被击飞才能被击倒
                if (Random.Range(0, 2) == 0 && !isDie && currentHealth > 0 && IsGrounded() && !isKnockback)
                {
                    Critial.SetActive(true);
                    Knockdown();
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


                    // 可以加一个简易翻面处理（仅左右）
                    if (StopX < 0)
                        Knockback(3);
                    else if (StopX > 0)
                        Knockback(-3);
                }





            }//格挡

            //伤害类型
            switch (TypeOfAttack)
            {
                case 1:
                    Strike_Effect.SetActive(true);//剑伤害
                    break;
                case 2:
                    //Palsy_Effect.SetActive(true);//雷电伤害
                    break;
            }



            currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
            UIManager.instance.UpdateHealthBar(currentHealth, maxHealth);

            //显示伤害
            HudText.HUD(amount);

            //有1秒左右的伤害冷却
            Invoke("HurtOver", 0.5f);
            RedScreen.SetActive(true);
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

            //血特效
            GameObject effectPrefabs = Instantiate(BloodEffect, transform.position, transform.rotation);
            Destroy(effectPrefabs, 2f);






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


        }//如果是已经Die了，那么这个淫乱槽不需要出现

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
    #endregion

}
