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
    public PatrolState patrolState = new PatrolState();//巡逻状态


    [Header("动画状态")]
    EnemyBaseState currentState;//当前状态
    public Animator anim;
    public int animState;

    [Header("基础属性")]
    public Rigidbody2D rb;
    public bool isDead = false;


    [Header("敌人攻击")]
    public GameObject attack_Collider;
    //public float attackRate;//攻击冷却
    //public float attackRange, skillRange;//攻击范围
    //float nextAttack = 0;

    public List<Transform> attackList = new List<Transform>();


    public AttackState attackState = new AttackState();//攻击状态

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



        currentState.OnUpdate(this);//每帧执行状态
        anim.SetInteger("state", animState);



    

    }

    public void TransitionToState(EnemyBaseState state)
    {
        currentState = state;
        currentState.EnterState(this);

    }//切换状态










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
        if (aiPath == null || enemyTarget == null)
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

    public void StopMove()
    {
        if (aiPath == null) return;

        aiPath.canMove = false;
        aiPath.maxSpeed = 0.01f;
    }

    private void SetNewPatrolTarget()
    {
        if (AreaManager.Instance == null)
        {
            StopMove();
            return;
        }

        currentPatrolPoint = AreaManager.Instance.GetRandomPatrolPoint();

        if (currentPatrolPoint == null)
        {
            StopMove();
            return;
        }

        enemyTarget.position = currentPatrolPoint.position;
    }//设置下一个巡逻目的地



    #endregion








}
