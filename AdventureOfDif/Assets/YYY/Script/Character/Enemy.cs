using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pathfinding;
using UnityEngine.InputSystem.Utilities;
using static GrabbableObject;



public enum EnemyState
{
    Idle,
    Moving,
    Attacking,
    Charging,    // 蓄力/冲锋
    Throwing,
    Grabbing,    // ✅ 抓住玩家（硬锁）
    Observing,   // ✅ 围观冻结
    Frozen,      // AVG 冻结
    Downed,
    Dead
}



public class Enemy : MonoBehaviour
{
    [Header("主动触发声音")]
    public FrameEvents frameEvents;

    [Header("寻找玩家/RoomGenerator")]
    [HideInInspector]
    public GameObject _Player;//玩家
    [HideInInspector]
    public Player player;
    [HideInInspector]
    public RoomGenerator RoomGenerator;//寻找RoomGenerator

    private void Start()
    {
        //找玩家
        _Player = GameObject.FindGameObjectWithTag("Player");
        player = _Player.GetComponent<Player>();

        //寻找RoomGenerator
        RoomGenerator = GameObject.FindGameObjectWithTag("RoomGenerator").GetComponent<RoomGenerator>();


        UpdateAllBar();//更新UI

        //速度岔开
        RunSpeed = Random.Range(3, 5);
        WalkSpeed = Random.Range(1, 3);


    }

    void FixedUpdate()
    {
        // if (isAVGFreeze) 
        // {
        //     //处于AVG中不能移动
        //     moveSpeed = 0;
        //     aiPath.maxSpeed = 0f;
        //     CleanupStatus();
        // }
        // else if (isRape)
        // {
        //
        //     //防止被更改状态的强制间断动画触发
        //     //RapeTimer += Time.deltaTime;
        //     //if (RapeTimer > 1f) { anim.Play("lewd"); RapeTimer = 0; }
        //
        //     //处于强奸中不能移动
        //     moveSpeed = 0;
        //     aiPath.maxSpeed = 0f;
        //
        //
        //     //非强奸中的其他敌人强制站立/清除冲锋状态
        //     if (_Player.GetComponent<Player>().enemyRaper != gameObject) 
        //     {
        //         CleanupStatus();
        //     }
        // }
        // else if (!isDie)
        // {
        //     BaseMove();//站走跑攻
        //
        //     //aiPath.canMove = true;
        //
        //
        // }
        // else
        // {
        //     //倒下后不能移动
        //     moveSpeed = 0;
        //     aiPath.maxSpeed = 0f;
        //
        //     //只要倒地就不显示
        //     attack_Collider.SetActive(false);
        //
        //
        //     // 只要到底，立即贴地
        //     Vector3 pos = transform.position;
        //     pos.y = groundY;
        //     transform.position = pos;
        //
        //     zHeight = 0f;
        //     zVelocity = 0f;
        //
        //     //aiPath.canMove = false;
        //
        //
        // }



        // 取动画机状态，避免与字段重名
        //AnimatorStateInfo animState = anim.GetCurrentAnimatorStateInfo(0);
        //
        //// 这些阶段一律禁走
        //bool blockByPhase = isDie || isRape || isAVGFreeze
        //                    || state == EnemyState.Grabbing
        //                    || state == EnemyState.Dead
        //                    // 技能‘准备’阶段禁走，冲锋阶段允许移动
        //                    || (state == EnemyState.Charging && isChargeAttack == 1)
        //                    // 投掷准备和出手一般也禁走（如果你想出手时可走就去掉这条）
        //                    || (state == EnemyState.Throwing);
        //
        //// 最终开关
        //aiPath.canMove = !blockByPhase;




        //当这些动画在播放的时候玩家不能移动
        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
        
        if (state.IsName("attack_1") ||
            state.IsName("attack_2") ||
            state.IsName("attack_3") ||
            state.IsName("attack_4") ||
            state.IsName("rage") ||
            state.IsName("hurt_1") ||
            state.IsName("hurt_2") ||
            state.IsName("block") ||
            state.IsName("fly") ||
        
        
        
            state.IsName("charge_hit") ||
            state.IsName("charge_ready") ||
        
            state.IsName("throw_ready") ||
            state.IsName("throw_out") ||
            state.IsName("stand_laugh") ||
        
            state.IsName("Down") ||
            state.IsName("down") ||
            state.IsName("down_getup") ||
            state.IsName("dead")
            )
        {
            aiPath.canMove = false;
        
        }
        else
        {
            aiPath.canMove = true;
        }









        // 取动画机状态，避免与字段重名
        //AnimatorStateInfo animState = anim.GetCurrentAnimatorStateInfo(0);

        // 最高优先：冻结/抓取/围观/死亡
        //if (this.state == EnemyState.Frozen || this.state == EnemyState.Grabbing || this.state == EnemyState.Observing)
        //{
        //    if (aiPath) { aiPath.canMove = false; aiPath.maxSpeed = 0f; }
        //    moveSpeed = 0;
        //    return;
        //}
        //if (this.state == EnemyState.Dead) return;


        // 最高优先级：冻结/抓取/死亡
        if (this.state == EnemyState.Frozen)
        {
            moveSpeed = 0;
            if (aiPath) { aiPath.maxSpeed = 0f; aiPath.canMove = false; }
            CleanupStatus();
            return;
        }

        if (this.state == EnemyState.Grabbing)
        {
            moveSpeed = 0;
            if (aiPath) { aiPath.maxSpeed = 0f; aiPath.canMove = false; }


            //非强奸中的其他敌人强制站立/清除冲锋状态
            if (_Player.GetComponent<Player>().enemyRaper != gameObject)
            {
                CleanupStatus();
            }

            return;
        }

        if (isDie || this.state == EnemyState.Dead)
        {
            return;
        }

        // 其余按原先分支
        BaseMove();


        UpdateShadow();//控制影子大小



        //始终跟随目标
        if (CurrentTarget != null)
        {
            _Target.transform.position = CurrentTarget.transform.position;

        }



        // 每帧更新剑物体的旋转
        Strike_Effect.transform.Rotate(0, 0, 100 * Time.deltaTime);




        
    }

    public bool isAVGFreeze = false;//处于AVG内部所有敌人停止
    public bool isRape = false;
    public bool isAttack = false;
    public bool isDie = false;


    /// <summary>
    /// 捕获系统
    /// </summary>
    #region
    [Header("捕获后的为了防止动画被更改")]
    public float RapeTimer;


   // public void CatchPlayer() 
   // {
   //     isRape = true;
   //     anim.Play("lewd");
   //
   //     gameObject.transform.position = _Player.transform.position;
   //     //shadow.transform.position = _Player.GetComponent<Player>().shadow.transform.position;
   //     shadow.gameObject.SetActive(false);//始终控制不好影子的位置
   //
   //
   //     _Player.GetComponent<Player>().shadow.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
   //
   //     _Player.GetComponent<Player>().characterSkin.HideSkeleton();
   //
   //
   //     _Player.GetComponent<Player>().isRape = true;
   //
   //     _Player.GetComponent<Player>().enemyRaper = this.gameObject;
   // }
   // public void ReleasePlayer()
   // {
   //     isRape = false;
   //     AnimBack(); // 或者回到待机动作
   //
   //     shadow.gameObject.SetActive(true);//始终控制不好影子的位置
   // }

    #endregion


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

    public AIPath aiPath;// A* 路径控制器




    [Header("速度岔开")]
    float RunSpeed = 4f;
    float WalkSpeed = 2f;



    private void BaseMove()
    {

        if (aiPath == null || !aiPath.hasPath) return;

        Vector2 current = transform.position;
        Vector2 target = aiPath.steeringTarget;

        Vector2 dir = (target - current).normalized;



        float dist = Vector2.Distance(current, target);


        if (isChargeAttack != 0)
        {
            if (IsGrounded()) 
            {
                ChargeAttack();
            }

        }
        else


       // if (isThrowAttack != 0)
       // {
       //     if (IsGrounded())
       //     {
       //         ThrowAttack();
       //     }
       //
       // }
       // else

        if (!isAttack)
        {
            // 设置速度与动画状态
            //if (dist > 1)
            //{
            //
            //    moveSpeed = 1;
            //    aiPath.maxSpeed = RunSpeed;
            //
            //}
            //else
            //{
            //    moveSpeed = 0;
            //    aiPath.maxSpeed = 0.01f;
            //}



            //让isAttack来决定移动还是攻击（目前这个同时测量距离currentTarget和isAttack可能是导致敌人站着不动的原因之一）
            moveSpeed = 1;
            aiPath.maxSpeed = RunSpeed;

        }
        else
        {
            BaseAttack();//攻击

            moveSpeed = 0;
            aiPath.maxSpeed = 0.01f;



        }


        CheckJump();
        //一旦target没有了就自动玩家
        //if (CurrentTarget == null)
        //{
        //    //CurrentTarget = _Player;
        //}


        if (isChargeAttack == 2 && LockTarget != null)
        {
            CurrentTarget = LockTarget.gameObject;


            if (Vector2.Distance(transform.position, LockTarget.position) < 1.5f)
            {
                isChargeAttack = 0;
                Destroy(LockTarget.gameObject);
                LockTarget = null;

                // 执行攻击动画/回到巡逻等逻辑
                anim.Play("charge_hit");

                //重置攻击状态
                //enemyVision_2.ResetChargeAttack();

            }
        }
        else
        {
            // 正常逻辑
            bool isLeft = transform.position.x < _Player.transform.position.x;
            CurrentTarget = isLeft ? player.Target_Right : player.Target_Left;
        }


        // 八方向判断（上下左右为主）
        if (dir.x > 0.5f)
        {
            inputX = 1; inputY = 0;
            attack.transform.rotation = Quaternion.Euler(0, 0, -90); // 右
        }
        else if (dir.x < -0.5f)
        {
            inputX = -1; inputY = 0;
            attack.transform.rotation = Quaternion.Euler(0, 0, 90); // 左
        }
        else if (dir.y > 0.5f)
        {
            inputX = 0; inputY = 1;
            attack.transform.rotation = Quaternion.Euler(0, 0, 0); // 上
        }
        else if (dir.y < -0.5f)
        {
            inputX = 0; inputY = -1;
            attack.transform.rotation = Quaternion.Euler(0, 0, 180); // 下
        }
        //else
        //{
        //    //inputX = 0; inputY = 0;
        //
        //    inputX = 0; inputY = -1;//朝正面
        //}

        // 储存方向用于 idle 状态
        if (inputX != 0 || inputY != 0)
        {
            StopX = inputX;
            StopY = inputY;
        }

        // 动画传入方向
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
    [Header("普通攻击")]


    public GameObject attack;//伤害朝向
    public GameObject attack_Collider;//伤害碰撞体


    //TODO：敌人站在玩家身边不动可能和这里有关系
    public float attackTimer = 0f;
    private float attackCooldown = 1f; // 原本 Invoke 的 1f
    public bool isInAttackDelay = false;

    //void BaseAttack()
    //{
    //
    //    //隔一会触发一下攻击
    //    if (!isInAttackDelay)
    //    {
    //        attackTimer += Time.deltaTime;
    //
    //        if (attackTimer >= attackCooldown)
    //        {
    //            Attack_Start(); // 攻击警告开始闪
    //
    //            attackTimer = 0f;
    //
    //            isInAttackDelay = true;
    //        }
    //
    //
    //    }
    //
    //
    //}






    //void Attack_Start()
    //{
    //
    //
    //    switch (Random.Range(0, 4))
    //    {
    //        case 0:
    //            anim.Play("attack_1", 0, 0);
    //            break;
    //        case 1:
    //            anim.Play("attack_2", 0, 0);
    //            break;
    //        case 2:
    //            anim.Play("attack_3", 0, 0);
    //            break;
    //        case 3:
    //            anim.Play("attack_4", 0, 0);
    //            break;
    //    }
    //
    //
    //    switch (Random.Range(0, 3))
    //    {
    //        case 0:
    //            frameEvents._Attack_sword_chop1();
    //            break;
    //        case 1:
    //            frameEvents._Attack_sword_chop2();
    //            break;
    //        case 2:
    //            frameEvents._Attack_sword_chop3();
    //            break;
    //    }
    //
    //    Invoke("Attack_Cancel", 1f);//一旦动画帧事件被跳过就会站着不动不攻击，所以这个还是Invoke触发
    //}


    public void Attack_Cancel()
    {
        isInAttackDelay = false;

    }

    [Header("蓄力攻击")]
    public int isChargeAttack = 0;//敌人蓄力冲过来  0没有触发  1蓄力握拳  2冲锋   
    public void ChargeAttack()
    {
        switch (isChargeAttack)
        {
            case 1:
                //蓄力握拳
                moveSpeed = 0;
                aiPath.maxSpeed = 0.01f;
                break;
            case 2:
                //冲锋
                moveSpeed = 2;
                aiPath.maxSpeed = 7f;
                anim.SetInteger("Speed", moveSpeed);
                break;
        }


    }

    public void CleanupStatus() 
    {
        if (currentHealth > 0) 
        {
            isChargeAttack = 0;

            anim.Play("stand");
        }

      

    }//强制回归初始状态


    [Header("远程攻击")]
    public GameObject Obstacle_Attack;
 
    public void ThrowHeldObject() 
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
            script.Launch(GrabbableType.Tanker);
        }

        enemyVision_2.isTrigger = false;//这里重置

    }

  

    #endregion


    /// <summary>
    /// 索敌系统
    /// </summary>
    #region
    [Header("索敌系统")]
    public GameObject _Target;//持续寻路对象

    [HideInInspector]
    public Transform LockTarget = null;//玩家原来站的点位

    public GameObject CurrentTarget;//当前的目标（这个会用于敌人攻击玩家原来站着的位置）

    //冲刺攻击重置
    public EnemyVision_2 enemyVision_2;


    #endregion



    /// <summary>
    /// 跳跃系统
    /// </summary>
    #region
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
            Debug.Log("跳跃");
            zVelocity = jumpForce;
            //frameEvents._SE_Clothes();
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
            if (wasInAir) // 刚刚落地的那一帧
            {
                //frameEvents._Effect_falldown();// 播放落地音效等逻辑
                Knockdown();
            }



            zHeight = 0f;
            zVelocity = 0f;
            groundY = transform.position.y;
        }


        if (zHeight > 0f)
        {
            Vector3 pos = transform.position;
            pos.y = groundY + zHeight;
            pos.z = -1f; // 跳跃时到前面
            transform.position = pos;

            //anim.SetBool("Jump", true);


        }
        else
        {
            Vector3 pos = transform.position;
            pos.z = 0f; // 落地恢复排序
            transform.position = pos;
            //anim.SetBool("Jump", false);
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

    public bool IsGrounded()
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


    [Header("被击飞")]
    float knockbackX = 0f; // 击飞时的水平速度（正负代表方向）

    public void Knockback(float force)
    {
        knockbackX = force; // 例如 -3 或 3
        zVelocity = jumpForce; // 同样上弹

        // 改变朝向
        if (knockbackX < 0)
            anim.gameObject.transform.localScale = new Vector3(-1, 1, 1);
        else if (knockbackX > 0)
            anim.gameObject.transform.localScale = new Vector3(1, 1, 1);
    }



    #endregion



    /// <summary>
    /// 生命值体力值等数值
    /// </summary>
    #region

    void UpdateAllBar()
    {
        //更新UI
        UpdateHealthBar(currentHealth, maxHealth);
    }

    [Header("特效")]
    public GameObject Strike_Effect;//剑光特效
    public GameObject Hit_Effect;//打击特效
    public GameObject BloodEffect;//受伤特效
    public GameObject SparkEffect;//火星特效

    [Header("生命值体力值等数值")]
    public int currentHealth;
    public int maxHealth;

    //伤害显示
    public bool isScreaming;
    public HudText HudText;



    public void ChangeHealth(int amount, int TypeOfAttack)//【攻击方式】  0轻攻击(打击特效)  1重攻击（击飞）(打击特效)  2剑击特效 
    {

        if (!isScreaming)
        {

            if (amount < 0)
            {
                //增加玩家暴击值
                player.ChangeCritical(100);


                if (Random.Range(0, 4) == 0 && !isDie && currentHealth > 0 && IsGrounded()) //（非死非空中）一定几率防御
                {
                    anim.Play("block");

                   // switch (Random.Range(0, 3))
                   // {
                   //     case 0:
                   //         frameEvents._Attack_sword_clash2();
                   //         break;
                   //     case 1:
                   //         frameEvents._Attack_sword_clash3();
                   //         break;
                   //     case 2:
                   //         frameEvents._Attack_sword_clash4();
                   //         break;
                   // }


                    //显示伤害
                    HudText.HUD(0);//0会显示Miss

                    return;
                }


                //击倒再站起(和暴击结合)只有站在地上才能被击倒
                if (Random.Range(0, 4) == 0 && !isDie && currentHealth > 0 && IsGrounded())
                {
                    Knockdown();
                }
                else
                {
                    if (!isDie)
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

                        //PlayJump();

                        if (TypeOfAttack == 0)
                        {

                            Invoke("AnimBack", 0.5f);//防止动画回不去

                            //受伤后重新计数（这在GetUp）
                            attackTimer = 0f;
                            isInAttackDelay = true;

                            //受伤后冲刺清零
                            isChargeAttack = 0;

                        }//被击飞
                        if (TypeOfAttack == 1)
                        {
                            // 可以加一个简易翻面处理（仅左右）
                            if (StopX < 0)
                                Knockback(3);
                            else if (StopX > 0)
                                Knockback(-3);
                        }//被击飞


                    }//处于倒地期间收到攻击不会触发受击动画



                }




                RoomGenerator.AddCombo();//连击显示
            }

            //伤害类型
            switch (TypeOfAttack)
            {
                case 0:
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
            UpdateHealthBar(currentHealth, maxHealth);

            //显示伤害
            HudText.HUD(amount);

            //有1秒左右的伤害冷却
            Invoke("HurtOver", 0.5f);

            isScreaming = true;

            //switch (Random.Range(0, 3))
            //{
            //    case 0:
            //        frameEvents._Attack_blood1();
            //        break;
            //    case 1:
            //        frameEvents._Attack_blood2();
            //        break;
            //    case 2:
            //        frameEvents._Attack_blood3();
            //        break;
            //}

            //血特效
            GameObject effectPrefabs = Instantiate(BloodEffect, transform.position, transform.rotation);
            Destroy(effectPrefabs, 2f);
        }


        if (currentHealth <= 0)
        {
            Die();

            return;
        }




    }
    void Fly()
    {
        if (!IsGrounded())
        {
            anim.Play("fly");
        }

    }

    void AnimBack()
    {
        if (currentHealth > 0) 
        {
            anim.Play("stand");
        }
         
    }

    void HurtOver()
    {
        isScreaming = false;
    }//有1秒左右的伤害冷却

    void GetUp()
    {
        if (currentHealth > 0 && IsGrounded())//双方只有在地上才能爬起
        {
            isDie = false;
            anim.Play("down_getup");
        }  //防止最后一下又击倒站起

    }//起身





    [Header("暴击")]
    public GameObject Critial;

    public void CritialAttack()
    {

        Knockdown();



        Time.timeScale = 0;

        //显示暴击
        Critial.SetActive(true);

        //暴击清零
        player.ChangeCritical(-player.maxCritical);

    }//暴击

    public void Knockdown()
    {


        isDie = true;
        anim.Play("down");

        if (currentHealth >= 0)
        {
            Invoke("GetUp", 1f);
        }  //防止最后一下又击倒站起

        //每次击倒后再站起来重新计算
        isInAttackDelay = false;
        attackTimer = 0f;

    }//击倒

    bool DieBonue = false;//死亡触发金币只能一次

    public void Die()
    {
        isDie = true;

      
        if (!DieBonue)
        {
            UIManager.instance.ChangeMoney(Random.Range(1, 10), true);

            DieBonue = true;
        }

        anim.Play("dead");//防止倒下又起来,搞了第二死亡

        Invoke("Disappear", 0.8f);
    }//死亡


    [Header("全部自身存在")]
    public GameObject AllOfThis;
    void Disappear()
    {
        Destroy(AllOfThis);

       // RoomGenerator.SetEnemy();

        Time.timeScale = 1;//防止 Critial消失之前次物体已经被毁坏，然后卡住不动了
    }




    [Header("生命值UI显示")]
    public Image HealthBar;
    public void UpdateHealthBar(int curAmount, int maxAmount)
    {
        HealthBar.fillAmount = (float)curAmount / (float)maxAmount;
    }//Enemy可能没有血条，但是Boss因该是要的所以先留着，替代UIManager的地方



    #endregion






    public EnemyState state = EnemyState.Idle;

    // 进入“硬锁”抓取：谁都不能打断
    public bool IsHardLocked => state == EnemyState.Grabbing || state == EnemyState.Dead || state == EnemyState.Frozen;

    // 统一判断接口：某个动作能不能开始
    public bool CanStart(ActionType act)
    {
        if (IsHardLocked) return false;           // 抓取 / 冻结 / 死亡直接不允许
        if (state == EnemyState.Downed) return false;

        switch (act)
        {
            case ActionType.Attack: return state == EnemyState.Idle || state == EnemyState.Moving;
            case ActionType.Charge: return state == EnemyState.Idle || state == EnemyState.Moving;
            case ActionType.Throw: return state == EnemyState.Idle || state == EnemyState.Moving;
            default: return false;
        }
    }

    public enum ActionType { Attack, Charge, Throw }

    // 统一中断一切非抓取行为
    void InterruptAllActions(string reason = "")
    {
        // 停移动
        moveSpeed = 0;
        if (aiPath) { aiPath.maxSpeed = 0f; aiPath.canMove = false; }

        // 清即时状态
        isAttack = false;
        isChargeAttack = 0;
        attackTimer = 0f;
        isInAttackDelay = false;

        // 取消所有计划中的回调/协程
        CancelInvoke();
        StopAllCoroutines();

        // 回到站立（避免残留动画层）
        if (currentHealth > 0) anim.Play("stand", 0, 0f);
    }

    public void CatchPlayer()
    {
        if (IsHardLocked) return;

        // 先中断自己
        InterruptAllActions("CatchPlayer");
        state = EnemyState.Grabbing;   // 抓取硬锁
        isRape = true;

        // 位置/显示省略…

        // 标记抓捕者
        player.isRape = true;
        player.enemyRaper = this.gameObject;

        // ✅ 现在再让其他敌人围观（避免第一帧状态抖动）
        //foreach (var e in GameObject.FindGameObjectsWithTag("Enemy"))
        //{
        //    var other = e.GetComponent<Enemy>();
        //    if (other && other != this) other.SetObserving(true);
        //}

        anim.Play("lewd", 0, 0);



        _Player.GetComponent<Player>().characterSkin.HideSkeleton();
        shadow.gameObject.SetActive(false);//始终控制不好影子的位置
        _Player.GetComponent<Player>().shadow.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
    }

    public void ReleasePlayer()
    {
        if (state != EnemyState.Grabbing) return;

        isRape = false;
        state = EnemyState.Idle;

        // 解除所有围观
        //foreach (var e in GameObject.FindGameObjectsWithTag("Enemy"))
        //{
        //    var other = e.GetComponent<Enemy>();
        //    if (other && other != this) other.SetObserving(false);
        //}

        // 恢复自己显示/移动…
        if (aiPath) aiPath.canMove = true;
        anim.Play("stand", 0, 0);



        shadow.gameObject.SetActive(true);//始终控制不好影子的位置
    }

















    void BaseAttack()
    {
        if (!CanStart(ActionType.Attack)) return;

        // 攻击 CD
        if (!isInAttackDelay)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackCooldown)
            {
                Attack_Start();
                attackTimer = 0f;
                isInAttackDelay = true;
            }
        }
    }

    void Attack_Start()
    {
        if (!CanStart(ActionType.Attack)) return;

        state = EnemyState.Attacking;

        anim.Play($"attack_{Random.Range(1, 5)}", 0, 0);

        // 声音
        //switch (Random.Range(0, 3))
        //{
        //    case 0: frameEvents._Attack_sword_chop1(); break;
        //    case 1: frameEvents._Attack_sword_chop2(); break;
        //    case 2: frameEvents._Attack_sword_chop3(); break;
        //}

        // ❗ 用协程替代 Invoke，且回调前再次校验（防止抓取期间旧回调乱入）
        StartCoroutine(EndAttackDelay(1f));
    }

    IEnumerator EndAttackDelay(float t)
    {
        yield return new WaitForSeconds(t);
        if (IsHardLocked || state == EnemyState.Grabbing || state == EnemyState.Dead) yield break;

        isInAttackDelay = false;
        if (state == EnemyState.Attacking) state = EnemyState.Idle;
    }











    public void StartCharge()
    {
        if (!CanStart(ActionType.Charge)) return;

        state = EnemyState.Charging;
        anim.Play("charge_ready", 0, 0);
        StartCoroutine(DoChargeAfter(1f)); // 替代 Invoke
    }
    IEnumerator DoChargeAfter(float delay)
    {
        float t = 0f;
        while (t < delay) { if (IsHardLocked) yield break; t += Time.deltaTime; yield return null; }

        if (!CanStart(ActionType.Charge)) yield break;

        isChargeAttack = 2;
        if (aiPath) aiPath.maxSpeed = 7f;
        anim.SetInteger("Speed", 2);

        // 锁定目标点
        var lockPoint = new GameObject("ChargeTarget");
        lockPoint.transform.position = player.transform.position;
        LockTarget = lockPoint.transform;
    }












    public void StartThrow()
    {
        if (!CanStart(ActionType.Throw)) return;

        state = EnemyState.Throwing;
        anim.Play("throw_ready", 0, 0);
        StartCoroutine(DoThrowAfter(1f));
    }

    IEnumerator DoThrowAfter(float delay)
    {
        float t = 0f;
        while (t < delay) { if (IsHardLocked) yield break; t += Time.deltaTime; yield return null; }

        if (!CanStart(ActionType.Throw)) yield break;

        anim.Play("throw_out", 0, 0);
    }




    public void SetObserving(bool on)
    {
        if (on)
        {
            state = EnemyState.Observing;
            isAttack = false;
            isChargeAttack = 0;
            CancelInvoke();
            StopAllCoroutines();
            if (aiPath) { aiPath.canMove = false; aiPath.maxSpeed = 0f; }
            anim.Play("stand", 0, 0);
        }
        else if (state == EnemyState.Observing)
        {
            state = EnemyState.Idle;
            if (aiPath) aiPath.canMove = true;
        }
    }
}

