using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVision_2 : MonoBehaviour
{
    public Enemy Enemy;
    public bool isTrigger = false;

    public int EnemyNumber;//1冲锋  2投掷

    // private void OnTriggerEnter2D(Collider2D collision)//检测到玩家显示
    // {
    //
    //     if (collision.gameObject.tag == "Player"&&!isTrigger)
    //     {
    //         if (Enemy.IsGrounded()&&Enemy.isRape==false) 
    //         {
    //             isTrigger = true;//先别再触发
    //
    //             switch (EnemyNumber) 
    //             {
    //                 case 1:
    //                     //进入蓄力状态1秒
    //                     Enemy.isChargeAttack = 1;
    //                     Enemy.anim.Play("charge_ready");
    //
    //                     //跑步冲锋状态
    //                     Invoke("Run", 1f);
    //                     break;
    //
    //                 case 2:
    //                     //进入蓄力状态1秒
    //                     Enemy.anim.Play("throw_ready");
    //
    //                     //投掷状态
    //                     Invoke("Throw", 1f);
    //                     break;
    //             }
    //
    //            
    //
    //
    //             
    //         }
    //        
    //     }//敌人攻击玩家
    //
    //
    // }
    //
    // private void Run()
    // {
    //
    //     if (Enemy.isRape == false) 
    //     {
    //         Enemy.isChargeAttack = 2;
    //
    //         // 锁定目标点（生成一个空物体在玩家当前位置）
    //         GameObject lockPoint = new GameObject("ChargeTarget");
    //         lockPoint.transform.position = Enemy.player.transform.position;
    //         Enemy.LockTarget = lockPoint.transform;
    //     }
    //
    //
    //    
    // }
    //
    // private void Throw() 
    // {
    //     if (Enemy.isRape == false)
    //     {
    //
    //         Enemy.anim.Play("throw_out");
    //     }
    //
    // }
    //
    // public void ResetChargeAttack() 
    // {
    //
    //     if (Enemy.isRape == false)
    //     {
    //         //重置状态
    //         Invoke("SetTrigger", 2f);
    //     }
    //
    //   
    // }
    //
    //
    // private void SetTrigger()
    // {
    //     if (Enemy.isRape == false)
    //     {
    //         if (Enemy.IsGrounded())
    //         {
    //             isTrigger = false;
    //
    //             Debug.Log("重置");
    //         }
    //         else
    //         {
    //             Invoke("SetTrigger", 2f);
    //         }
    //     }
    //
    //
    //
    //   
    //    
    // }//如果在空中就别重置，再等2秒







    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player") || isTrigger) return;
        if (!Enemy.IsGrounded() || Enemy.IsHardLocked) return;

        isTrigger = true;

        switch (EnemyNumber)
        {
            case 1: Enemy.StartCharge(); Invoke("Run", 1f); break;
            case 2: Enemy.StartThrow(); Invoke("Throw", 1f); break;
        }
    }


    private void Run()
    {
        if (Enemy.IsHardLocked) return;

        Enemy.isAttack = false;                 // 防止小圈的普攻抢控制权
        Enemy.state = EnemyState.Charging;
        Enemy.isChargeAttack = 2;               // 阶段=冲锋

        if (Enemy.aiPath)
        {
            Enemy.aiPath.canMove = true;        // 冲锋阶段允许移动
            Enemy.aiPath.maxSpeed = 7f;
        }

        Enemy.anim.Play("charge_run", 0, 0f);   // ⚠️ 切到可移动的动画（自己项目里的名字）

        // 锁定目标点
        GameObject lockPoint = new GameObject("ChargeTarget");
        lockPoint.transform.position = Enemy.player.transform.position;
        Enemy.LockTarget = lockPoint.transform;
    }

    private void Throw()
    {
        if (Enemy.IsHardLocked) return;

        Enemy.isAttack = false;
        Enemy.state = EnemyState.Throwing;
        Enemy.anim.Play("throw_out", 0, 0f);     // 出手动画

        // 建议：用动画事件调用 Enemy.ThrowHeldObject()
        // 或者这里用协程延时并二次校验：
        StartCoroutine(DoThrowAfter(0.1f));
    }
    private IEnumerator DoThrowAfter(float t)
    {
        yield return new WaitForSeconds(t);
        if (Enemy.IsHardLocked || Enemy.state != EnemyState.Throwing) yield break;
        Enemy.ThrowHeldObject();
    }
}
