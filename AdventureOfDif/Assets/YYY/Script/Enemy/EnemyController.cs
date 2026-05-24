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

    [Header("基础属性")]
    public Rigidbody2D rb;
    public bool isDead = false;
    public bool isHurt = false;//为了受伤的时候屏蔽Update

    [Header("敌人攻击")]
    public GameObject attack_Collider;
    public float attackRate;//攻击冷却
    public float attackRange;//攻击范围
    float nextAttack = 0;

    public List<Transform> attackList = new List<Transform>();


    public PatrolState patrolState = new PatrolState();//巡逻状态
    public AttackState attackState = new AttackState();//攻击状态
    public HitState hitState = new HitState();//受击状态

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

        if (RoomGenerator.instance != null && RoomGenerator.instance.gameOver)
        {
            attackList.Clear();
            targetPoint = null;
            anim.ResetTrigger("attack");
            anim.ResetTrigger("skill");
            animState = 0;
            anim.SetInteger("state", animState);
            return;
        }//玩家死后强制停战

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

    public void TransitionToState(EnemyBaseState state)
    {
        currentState = state;
        currentState.EnterState(this);

    }//切换状态





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
        Debug.Log("冲向目标！");

    }//锁定目标







    public void AttackAction()
    {
        Debug.Log("攻击！");

        if (targetPoint == null) return;

        float distance = Vector2.Distance(transform.position, targetPoint.position);

        if (distance <= attackRange)
        {
            StopMove();//攻击近敌停
            FilpDirection();

            if (Time.time > nextAttack)
            {
                anim.SetTrigger("attack");
                Debug.Log("普通攻击");
                nextAttack = Time.time + attackRate;
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
        if (aiPath == null || enemyTarget == null|| isHurt || isDead)
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
        isHurt = false;

        aiPath.canMove = true;
        aiPath.maxSpeed = 1.5f;

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



    #endregion



    /// <summary>
    /// 受伤死亡
    /// </summary>
    #region
    [Header("受伤死亡")] 
    public GameObject Effect_Blood;

    [Header("主动触发声音")]
    public FrameEvents frameEvents;

  

    public void OnTakeDamage(Attack attack)
    {

        if (isDead)
        {
            return;
        }
        if (attack == null)
            return;


      


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


        anim.SetTrigger("hit");
        anim.SetInteger("HitType",Random.Range(1,3));

        if (attack.clearVelocity)
        {
            rb.velocity = Vector2.zero;
        }

        float dir = transform.position.x >= attack.transform.position.x ? 1f : -1f;

        rb.AddForce(
            new Vector2(dir * attack.knockbackX, attack.knockbackY),
            ForceMode2D.Impulse
        );


        PlayBloodEffect();

        isHurt = true;
        TransitionToState(hitState);//进入受击状态
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
        PlayBloodEffect();

        isDead = true;
        StopMove();//死亡停


        SetStateColor(deadColor);
    }


    #endregion



    [Header("状态显示器")]
    public SpriteRenderer stateViewer;

    public Color patrolColor = Color.green;
    public Color attackColor = Color.red;
    public Color hitColor = Color.yellow;
    public Color deadColor = Color.gray;

    public void SetStateColor(Color color)
    {
        if (stateViewer != null)
            stateViewer.color = color;
    }
}
