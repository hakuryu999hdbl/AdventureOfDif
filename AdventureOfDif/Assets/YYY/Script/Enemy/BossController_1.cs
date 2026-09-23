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

    [Header("阶段2")]
    public float phase2HealthRate = 0.7f;

    public GameObject[] minionPrefabs;
    public Transform[] minionSpawnPoints;

    private List<GameObject> aliveMinions = new List<GameObject>();

    private bool minionsSpawned = false;

    private bool phase2Started;


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
        if (phase2Started)
            return;

        float healthRate =
            bossCharacter.currentHealth /
            bossCharacter.maxHealth;

        if (healthRate <= phase2HealthRate)
        {
            StartPhase2();
        }
    }


    // =========================
    // Phase 2
    // =========================

    void StartPhase2()
    {
        phase2Started = true;
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
        // ★恢复动画器出生时的局部位置
        boss.anim.transform.localPosition = boss.animOriginalLocalPosition;

        // Boss播放从卡车跳回来的动画
        boss.anim.SetTrigger("jumpIn");


       
    }


    public void FinishPhase2()
    {
        boss.isBossAir = false;

        boss.shadow.gameObject.SetActive(true);

        currentPhase = BossPhase.Phase1_Battle;

        boss.EnterBattleState();

       // Debug.Log("★★ Boss返回战斗");
    }


    [Header("Boss演出相机")]
    public CameraControl cameraControl;




    public PlayerController player;

    private void Start()
    {
        player = RoomGenerator.instance.player;
        cameraControl = RoomGenerator.instance.cameraControl;
    }





}
