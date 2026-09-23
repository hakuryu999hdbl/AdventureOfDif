using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss_1 : EnemyController
{


    [Header("Boss弹幕爆炸")]
    public GameObject Barrage;


    public void ShowBarrage() 
    {

        Barrage.SetActive(true);

    }


    [Header("Boss动画器移动")]
    public BossController_1 BossController_1;
    public  Vector3 animOriginalLocalPosition;//动画原始位置

    [Header("Boss特殊阶段")]
    public bool isBossAir = false;


    // jump_In 帧事件
    // Boss跳上卡车
    public void MoveBossToTruck()
    {
 

        anim.gameObject.transform.position = BossController_1.truckPoint.transform.position;



        //镜头拉相机
        BossController_1.cameraControl.SetFollowTarget(BossController_1.truckPoint);

        Invoke(nameof(SunmonEnemy), 1f);
    }

    void SunmonEnemy() 
    {

        //召唤敌人
        BossController_1.SpawnMinions();
    }



    // jump_Out 帧事件
    // Boss回到战斗场地
    public void MoveBossToBattle()
    {
     

        BossController_1.FinishPhase2();


    }













    [Header("格挡系统")]
    public float maxBlockValue = 50f;
    public float currentBlockValue = 50f;

    [Tooltip("每次受到攻击消耗的基础格挡值")]
    public float blockCostMultiplier = 35f;



    [Header("格挡UI")]
    public GameObject blockBarRoot;
    public Image blockBarFill;
    public GameObject Text_Block;//弹出防御提示


    [Header("格挡状态")]
    public bool isBlocking;
    public bool isCountering;


    public override void Init()
    {
        base.Init();

        // ★记录动画器最初的位置
        animOriginalLocalPosition = anim.transform.localPosition;

        currentBlockValue = maxBlockValue;
        UpdateBlockUI();
    }

    public override void Update()
    {
        // Boss处于跳出场地/卡车/演出状态
        if (isBossAir)
        {
            StopMove();
            return;
        }



        base.Update();

        UpdateBlockRecovery();
    }
    public override bool IgnoreIncomingDamage()
    {
        return isBossAir;
    }
    public override bool TryHandleIncomingAttack(Attack attack)
    {
        //Debug.Log("摩托哥收到攻击，检查格挡");

        if (attack == null)
        {
            Debug.Log("格挡失败：attack为空");
            return false;
        }

        //Debug.Log(
        //    $"攻击前：maxBlock={maxBlockValue}，" +
        //    $"currentBlock={currentBlockValue}，" +
        //    $"blockDamage={attack.damage}，" +
        //    $"dead={isDead}，hurt={isHurt}，broken={isBlockBroken}"
        //);

        if (isDead)
        {
            //Debug.Log("格挡失败：敌人死亡");
            return false;
        }

        if (isHurt)
        {
            //Debug.Log("格挡失败：敌人已经受击");
            return false;
        }

        currentBlockValue -= attack.damage;
        currentBlockValue = Mathf.Max(0f, currentBlockValue);

        Debug.Log($"扣除后格挡值：{currentBlockValue}");

        UpdateBlockUI();

        if (currentBlockValue <= 0f)
        {
            Debug.Log("本次攻击直接打空格挡，没有进入BlockState");


            isBlocking = false;
            isCountering = false;

            anim.SetInteger("skillState", 0);

            return false;
        }

        Debug.Log("准备进入BlockState");

        TransitionToState(blockState);

        Debug.Log("TransitionToState已经调用");

        return true;
    }



    private void UpdateBlockRecovery()
    {
        if (isDead || isHurt)
            return;

        if (isBlocking || isCountering)
            return;



        if (currentBlockValue >= maxBlockValue)
        {
            currentBlockValue = maxBlockValue;

            UpdateBlockUI();
            return;
        }



        currentBlockValue =
            Mathf.Min(currentBlockValue, maxBlockValue);


        UpdateBlockUI();
    }

    private void UpdateBlockUI()
    {
        blockBarFill.fillAmount =
                 currentBlockValue / maxBlockValue;

        if (currentBlockValue <= 0f)
        {
            blockBarRoot.SetActive(false);
            return;

        }//只要没防御就隐藏

        blockBarRoot.SetActive(
                 currentBlockValue < maxBlockValue ||
                 isBlocking
             );
    }

    // 防御动画事件：
    // 举起武器后开始反击
    public void StartCounterAttack()
    {
        if (isDead || isHurt)
            return;

        StopMove();

        isBlocking = false;
        isCountering = true;

        anim.SetInteger("skillState", 2);

        if (targetPoint != null)
            FaceToPosition(targetPoint.position);
    }

    // 反击动画结束事件
    public void EndCounterAttack()
    {
        isBlocking = false;
        isCountering = false;

        anim.SetInteger("skillState", 0);

        if (isDead)
            return;

        if (attackList.Count > 0)
            TransitionToState(attackState);
        else
            TransitionToState(patrolState);
    }
}
