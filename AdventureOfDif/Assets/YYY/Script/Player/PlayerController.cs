using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    [Header("基础属性")]
    //public Character character;

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
    public GameObject attack_Collider;


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
        if (Time.time > nextAttack)
        {


            nextAttack = Time.time + attackRate;
        }
    }
    void PlayerAttack(InputAction.CallbackContext obj)
    {





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
