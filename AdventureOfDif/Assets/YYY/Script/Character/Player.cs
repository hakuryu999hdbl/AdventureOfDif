using System.Collections;
using System.Collections.Generic;
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

        if (!isDie)
        {
            BaseMove();//站走跑攻


        }
        else
        {
            rbody.velocity = Vector2.zero; // 停止所有移动
        }

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
            state.IsName("hurt_2"))
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
                PlayNormalAttack(); // 普通攻击
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
    public  int currentCombo = 0;
    public bool isAttacking2 = false;
    public bool canCombo = false;
    public bool comboQueued = false;


    public void PlayNormalAttack()
    {

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
        isRunning = true;
    }
    private void OnRunCanceled(InputAction.CallbackContext context)
    {
        isRunning = false;
    }

    private void OnAttackStarted(InputAction.CallbackContext context)
    {
        Attack_Start();
    }
    private void OnAttackCanceled(InputAction.CallbackContext context)
    {
        Attack_Cancel();
    }

    private void OnDodgeStarted(InputAction.CallbackContext context)
    {
        
    }
    private void OnDodgeCanceled(InputAction.CallbackContext context)
    {
        
    }

    [Header("手机端触发")]
    public Joystick Joystick;

    //手机端触发
    public bool isRunning = false;//持续按下跑步键
    public void ButtonSetRun()
    {
        isRunning = true;
    }
    public void ButtonSetStop()
    {
        isRunning = false;
    }

    //手机端触发
    public bool isAttacking = false;//持续按下攻击键
    public void ButtonSetAttack()
    {
        Attack_Start();
    }
    public void ButtonSetAttackOver()
    {
        Attack_Cancel();
    }

    //手机端触发
    public bool isDodging = false;//持续按下闪避键
    public void ButtonSetDodge()
    {
        
    }
    public void ButtonSetDodgeOver()
    {

        
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
        if (!isScreaming)
        {




            if (amount < 0)
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


                //if (!isDie&&canMove)//处于攻击状态下无法防御
                //{
                //    // 计算体力百分比
                //    float strengthPercent = (float)currentStrength / maxStrength;
                //
                //    // 根据体力百分比决定防御几率（体力越高越容易防御）
                //    // 比如体力满时为 100% 几率，体力最低时为 10%
                //    float blockChance = Mathf.Lerp(0.1f, 1f, strengthPercent);
                //
                //    if (Random.value < blockChance)
                //    {
                //        anim.SetTrigger("Block");
                //
                //        // 防御成功扣除体力
                //        ChangeStrength(-50);
                //
                //
                //        switch (Random.Range(0, 3))
                //        {
                //            case 0:
                //                frameEvents._Attack_sword_clash2();
                //                break;
                //            case 1:
                //                frameEvents._Attack_sword_clash3();
                //                break;
                //            case 2:
                //                frameEvents._Attack_sword_clash4();
                //                break;
                //        }
                //
                //
                //        //显示伤害
                //        HudText.HUD(0);//0会显示Miss
                //
                //        //火花特效
                //        Vector3 offset_2 = new Vector3(0, 0, 2); // 这里的1表示沿Z轴上升的距离，可以根据需要调整
                //        Vector3 spawnPosition_2 = transform.position + offset_2;
                //        GameObject effectPrefabs_2 = Instantiate(SparkEffect, spawnPosition_2, transform.rotation);
                //        Destroy(effectPrefabs_2, 2f);
                //
                //        return;
                //    }
                //
                //}

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



            //受伤时连击取消
            Invoke("ResetCombo", 1f);



            if (currentHealth <= 0)
            {
                isDie = true;



                anim.Play("dead");

                return;
            }

            //击倒再站起
            if (Random.Range(0, 2) == 0 && !isDie && currentHealth > 0)
            {
                isDie = true;

                anim.Play("down");

                //防止最后一下又击倒站起
                if (currentHealth > 0)
                {
                    Invoke("GetUp", 0.5f);//比起敌人，玩家可以更快站起来
                }

                Critial.SetActive(true);
            }

        }

    }

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

        // 判定是否暴击
        if (Random.value < critRate)
        {
            strike.isCritial = true;

            // 扣除暴击值
            ChangeCritical(-maxCritical); // 或者换成一部分
        }
        else
        {
            strike.isCritial = false;
        }
    }
    #endregion

}
