using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class BossController_1 : MonoBehaviour
{

 
    public enum BossPhase
    {
        Phase1_Battle,
        Phase2_Summon,
        Phase3_Barrage,
        Dead
    }

    [Header("Boss位置")]
    public Transform truckPoint;

    [Header("Boss")]
    public Boss_1 boss;
    public Character bossCharacter;
    public Transform BossTransform;

    [Header("阶段")]
    public BossPhase currentPhase = BossPhase.Phase1_Battle;



    [Header("阶段循环")]
    public float[] phaseHealthRates = { 0.7f, 0.3f };

    // 当前已经触发到第几次血量循环
    private int phaseCycleIndex = 0;

    // 当前这个 Phase2 是循环里的第几个 2
    // 0 = 1后面的2
    // 1 = 3后面的2
    private int phase2Step = 0;




    public GameObject[] minionPrefabs;
    public Transform[] minionSpawnPoints;

    private List<GameObject> aliveMinions = new List<GameObject>();

    private bool minionsSpawned = false;



    [Header("阶段3 弹幕")]
    public Transform barrageLeftPoint;
    public Transform barrageRightPoint;

    public int barrageRoundCount = 4;

    private int currentBarrageRound;
    private bool barrageGoingRight;



    private void Update()
    {
        if (boss == null || bossCharacter == null)
            return;

        switch (currentPhase)
        {
            case BossPhase.Phase1_Battle:

                CheckPhase1();
                break;


            case BossPhase.Phase2_Summon:

                CheckMinions();
                break;
        }
    }


    // =========================
    // Phase 1
    // =========================
    void CheckPhase1()
    {
        // 两次血量阶段都已经触发完
        if (phaseCycleIndex >= phaseHealthRates.Length)
            return;

        float healthRate =
            bossCharacter.currentHealth /
            bossCharacter.maxHealth;

        if (healthRate <= phaseHealthRates[phaseCycleIndex])
        {
            // 进入这一轮的第一个 Phase2
            phase2Step = 0;

            // 先推进循环，防止回来后再次触发同一个血量点
            phaseCycleIndex++;

            StartPhase2();
        }
    }


    // =========================
    // Phase 2
    // =========================
    void StartPhase2()
    {

        currentPhase = BossPhase.Phase2_Summon;


        // 玩家不能操作
        player.DisableGameplayInput();
        player.rb.velocity = Vector2.zero;

        // 镜头看Boss
        cameraControl.SetFollowTarget(BossTransform);



        // ★Boss进入场外状态
        boss.isBossAir = true;
        boss.shadow.gameObject.SetActive(false);

        //★Boss清理自身巡逻/攻击/技能状态
        boss.CleanState();

        // Boss停止战斗
        boss.StopMove();

        // ★强制结束当前受击/击飞动画
        boss.ClearHitState();


        // ★Skill Layer开始跳走
        boss.anim.SetTrigger("jumpOut");

        minionsSpawned = false;
    }

    // =========================
    // Phase 3
    // =========================
    public void StartPhase3()
    {
        currentPhase = BossPhase.Phase3_Barrage;

        boss.isBossAir = true;

        boss.CleanState();
        boss.StopMove();
        boss.ClearHitState();

        currentBarrageRound = 0;

        // 第一站左边
        barrageGoingRight = false;

        boss.anim.SetTrigger("barrageJumpOut");
    }

    public void MoveBossToBarragePoint()
    {
        Transform point = barrageGoingRight
            ? barrageRightPoint
            : barrageLeftPoint;

        // ★整个Boss移动
        boss.transform.position = point.position;

        // ★动画器恢复自己正常的局部位置
        boss.anim.transform.localPosition =
            boss.animOriginalLocalPosition;


        // ★右边：朝左
        if (barrageGoingRight)
        {
            boss.transform.localScale =
                new Vector3(-1, 1, 1);
        }
        // ★左边：朝右
        else
        {
            boss.transform.localScale =
                new Vector3(1, 1, 1);
        }


    }
    public void BarrageAttackOver()
    {
        if (currentPhase != BossPhase.Phase3_Barrage)
            return;

        currentBarrageRound++;

        // 达到次数，结束Phase3
        if (currentBarrageRound >= barrageRoundCount)
        {
            EndPhase3();
            return;
        }

        // 下一次换另一边
        barrageGoingRight = !barrageGoingRight;

        // 再次跳走
        boss.anim.SetTrigger("barrageJumpOut");
    }




    private void StartNextBarrage()
    {
        if (currentPhase != BossPhase.Phase3_Barrage)
            return;

        boss.anim.SetTrigger("barrageJumpOut");
    }




    public void SpawnMinions()
    {
        aliveMinions.Clear();

        for (int i = 0; i < minionPrefabs.Length; i++)
        {
            Transform point =
                minionSpawnPoints[i % minionSpawnPoints.Length];

            GameObject enemy = Instantiate(
                minionPrefabs[i],
                point.position,
                Quaternion.identity
            );

            aliveMinions.Add(enemy);
        }

        // ★确定真的召唤过了
        minionsSpawned = true;

        // ★镜头还给玩家
        cameraControl.FollowPlayer();

        // ★恢复操作
        player.EnableGameplayInput();
    }


    void CheckMinions()
    {
        // ★Boss还没完成召唤，不检测“全灭”
        if (!minionsSpawned)
            return;

        aliveMinions.RemoveAll(enemy => enemy == null);

        if (aliveMinions.Count == 0)
        {
            EndPhase2();
        }
    }


    void EndPhase2()
    {
        // 防止 Update 每帧重复调用
        minionsSpawned = false;

        // 恢复动画器出生时的局部位置
        boss.anim.transform.localPosition =
            boss.animOriginalLocalPosition;

        // Boss从卡车跳回场地
        boss.anim.SetTrigger("jumpIn");
    }


    void EndPhase3()
    {
        StartPhase2();
    }


    public void FinishPhase2()
    {
        boss.isBossAir = false;
        boss.shadow.gameObject.SetActive(true);

        // 第一轮2结束
        // 1 → 2 → 3
        if (phase2Step == 0)
        {
            phase2Step = 1;

            StartPhase3();
            return;
        }

        // 第二轮2结束
        // 3 → 2 → 1
        currentPhase = BossPhase.Phase1_Battle;

        boss.EnterBattleState();
    }


    [Header("Boss演出相机")]
    public CameraControl cameraControl;




    public PlayerController player;

    private void Start()
    {
        player = RoomGenerator.instance.player;
        cameraControl = RoomGenerator.instance.cameraControl;




        // ★测试：开场直接进入弹幕阶段
        //Invoke(nameof(StartPhase3), 1f);
    }





}
