using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pathfinding;
using UnityEngine.InputSystem.Utilities;

public class Enemy : MonoBehaviour
{
    [Header("主动触发声音")]
    public FrameEvents frameEvents;

    [Header("寻找玩家/RoomGenerator")]
    public GameObject _Player;//玩家
    public Player player;

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
        if (!isDie)
        {
            BaseMove();//站走跑攻




        }
        else
        {
            //倒下后不能移动
            moveSpeed = 0;
            aiPath.maxSpeed = 0f;

            //只要倒地就不显示
            attack_Collider.SetActive(false);


        }







        //始终跟随目标
        if (CurrentTarget != null)
        {
            _Target.transform.position = CurrentTarget.transform.position;

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
            aiPath.canMove = false;

        }
        else
        {
            aiPath.canMove = true;
        }
    }

    public bool isAttack = false;
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

        if (!isAttack)
        {
            // 设置速度与动画状态
            if (dist > 1)
            {


                if (tag != "Friend")
                {
                    //目前战斗下全员跑
                    moveSpeed = 2;
                    aiPath.maxSpeed = RunSpeed;
                }
                else
                {
                    //非巡逻队友跟，随情况下会你走/我也走/你跑/我也跑

                    if (player.isRunning)
                    {
                        moveSpeed = 2;
                        aiPath.maxSpeed = RunSpeed;
                    }
                    else
                    {

                        moveSpeed = 1;
                        aiPath.maxSpeed = WalkSpeed;

                    }

                }







            }
            else
            {
                moveSpeed = 0;
                aiPath.maxSpeed = 0.01f;
            }


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


        bool isLeft = transform.position.x < _Player.transform.position.x;
        CurrentTarget = isLeft ? player.Target_Right : player.Target_Left;


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
    [Header("蓄力攻击")]


    public GameObject attack;//伤害朝向
    public GameObject attack_Collider;//伤害碰撞体



    private float attackTimer = 0f;
    private float attackCooldown = 1f; // 原本 Invoke 的 1f
    private bool isInAttackDelay = false;

    void BaseAttack()
    {

        //隔一会触发一下攻击
        if (!isInAttackDelay)
        {
            attackTimer += Time.deltaTime;

            if (attackTimer >= attackCooldown)
            {
                Attack_Start(); // 攻击警告开始闪

                attackTimer = 0f;


                isInAttackDelay = true;
            }


        }


    }






    void Attack_Start()
    {
        anim.Play("attack_1", 0, 0);



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

        Invoke("Attack_Cancel", 1f);//一旦动画帧事件被跳过就会站着不动不攻击，所以这个还是Invoke触发
    }


    public void Attack_Cancel()
    {
        isInAttackDelay = false;

    }

    #endregion




    /// <summary>
    /// 巡逻系统
    /// </summary>
    #region
    [Header("索敌系统")]
    public GameObject _Target;//持续寻路对象

    public GameObject CurrentTarget;//当前的目标
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
            if (wasInAir) // 刚刚落地的那一帧
            {
                frameEvents._Effect_falldown();// 播放落地音效等逻辑
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
            transform.position = pos;

            //anim.SetBool("Jump", true);
        }
        else
        {
            //anim.SetBool("Jump", false);
        }

        // 更新前一帧状态
        wasInAir = !isGroundedNow;


        UpdateShadow();//控制影子大小


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
    public GameObject BloodEffect;//受伤特效
    public GameObject SparkEffect;//火星特效

    [Header("生命值体力值等数值")]
    public int currentHealth;
    public int maxHealth;

    //伤害显示
    public bool isScreaming;
    public HudText HudText;



    public void ChangeHealth(int amount, int TypeOfAttack)//【攻击方式  0无  1剑击特效  2闪电特效  3冻结
    {

        if (!isScreaming)
        {




            if (amount < 0)
            {




                //if (Random.Range(0, 3) == 0 && !isDie && currentHealth > 0 && amount != -currentHealth)
                //{
                //    anim.SetTrigger("Block");
                //
                //    switch (Random.Range(0, 3))
                //    {
                //        case 0:
                //            frameEvents._Attack_sword_clash2();
                //            break;
                //        case 1:
                //            frameEvents._Attack_sword_clash3();
                //            break;
                //        case 2:
                //            frameEvents._Attack_sword_clash4();
                //            break;
                //    }
                //
                //    //显示伤害
                //    HudText.HUD(0);//0会显示Miss
                //
                //    //火花特效
                //    Vector3 offset_2 = new Vector3(0, 0, 2); // 这里的1表示沿Z轴上升的距离，可以根据需要调整
                //    Vector3 spawnPosition_2 = transform.position + offset_2;
                //    GameObject effectPrefabs_2 = Instantiate(SparkEffect, spawnPosition_2, transform.rotation);
                //    Destroy(effectPrefabs_2, 2f);
                //
                //    return;
                //}


                //击倒再站起(和暴击结合)只有站在地上才能被击倒
                if (Random.Range(0, 2) == 0 && !isDie && currentHealth > 0 && IsGrounded())
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

                        //PlayJump();


                        // 可以加一个简易翻面处理（仅左右）
                        if (StopX < 0)
                            Knockback(3);
                        else if (StopX > 0)
                            Knockback(-3);

                    }//处于倒地期间收到攻击不会触发受击动画


                  
                }
            }

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
            UpdateHealthBar(currentHealth, maxHealth);

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

    public void Die()
    {
        isDie = true;
        anim.Play("dead");//防止倒下又起来,搞了第二死亡

        Invoke("Disappear", 1f);
    }//死亡


    [Header("全部自身存在")]
    public GameObject AllOfThis;
    void Disappear()
    {
        Destroy(AllOfThis);

        RoomGenerator.SetEnemy();

        Time.timeScale = 1;//防止 Critial消失之前次物体已经被毁坏，然后卡住不动了
    }




    [Header("生命值UI显示")]
    public Image HealthBar;
    public void UpdateHealthBar(int curAmount, int maxAmount)
    {
        HealthBar.fillAmount = (float)curAmount / (float)maxAmount;
    }//Enemy，Friend，NPC替代UIManager的地方


    #endregion




    /// <summary>
    /// 阵营转换
    /// </summary>
    #region
    [Header("阵营转换")]
    public EnemyVision vision;
    public EnemyVision vision_2;
    public Strike strike;
    public Image HealthValueImage;

    //切换为队友
    public void ConvertToFriend()
    {
        //  修改标签
        this.tag = "Friend";

        //  视野脚本：变成队友
        vision.isFriend = true;

        //  视野脚本2：变成队友
        vision_2.isFriend = true;

        //  攻击脚本：攻击敌人，不再攻击队友
        strike.DamageToPlayer = false;
        strike.DamageToEnemy = true;
        strike.DamageToFriend = false;

        //  改变血条颜色为绿色（友军色）
        HealthValueImage.color = Color.green;

        attack_Collider.GetComponent<SpriteRenderer>().color = Color.green;


        Debug.Log($"{gameObject.name} has switched to Friend.");
    }

    // 切换为敌人
    public void ConvertToEnemy()
    {
        // 修改标签
        this.tag = "Enemy";

        // 视野脚本：不是队友
        vision.isFriend = false;

        // 攻击脚本：攻击玩家和友军，不攻击敌人
        strike.DamageToPlayer = true;
        strike.DamageToEnemy = false;
        strike.DamageToFriend = true;

        // 改变血条颜色为红色（敌人色）
        HealthValueImage.color = Color.red;



        attack_Collider.GetComponent<SpriteRenderer>().color = Color.red;

        Debug.Log($"{gameObject.name} has switched to Enemy.");
    }
    #endregion





}

