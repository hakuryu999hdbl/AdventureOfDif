using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enemy;

public class EnemyVision : MonoBehaviour
{
    public Enemy Enemy;




    // private void OnTriggerStay2D(Collider2D collision)//检测到玩家显示
    // {
    //
    //
    //     if (collision.gameObject.tag == "Player")
    //     {
    //         if (collision.gameObject.GetComponent<Player>().isDie && Enemy.isRape == false && Enemy.isDie == false)
    //         {
    //             if (collision.gameObject.GetComponent<Player>().isRape == false)
    //             {
    //               
    //
    //                 // 寻找场景中所有其他敌人，设置围观
    //                 GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
    //                 foreach (GameObject e in allEnemies)
    //                 {
    //                     Enemy other = e.GetComponent<Enemy>();
    //                     if (other != null && other != Enemy && !Enemy.player.observingEnemies.Contains(other))
    //                     {
    //                         other.isRape = true; // 进入围观状态
    //                         Enemy.player.observingEnemies.Add(other);
    //
    //                     }
    //                 }
    //
    //                 Enemy.CatchPlayer();
    //             }
    //
    //         }
    //         else
    //         {
    //             Enemy.isAttack = true;
    //         }
    //
    //
    //
    //
    //     }//敌人攻击玩家
    //
    //
    //
    //
    //
    //
    // }
    //
    // private void OnTriggerExit2D(Collider2D collision)//检测到玩家显示
    // {
    //     if (collision.gameObject.tag == "Player")
    //     {
    //         Enemy.isAttack = false;
    //
    //     }//敌人停止攻击玩家
    //
    //
    // }



    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        var pl = collision.GetComponent<Player>();
        if (!pl) return;

        // 玩家倒地 且 敌人不在硬锁 → 抓取
        if (pl.isDie && !Enemy.IsHardLocked && !pl.isRape)
        {

            Enemy.CatchPlayer(); // 统一入口
            return;
        }


        // 技能/抓取/冻结阶段，不要把 isAttack 置真
        if (Enemy.state == EnemyState.Charging ||
            Enemy.state == EnemyState.Throwing ||
            Enemy.state == EnemyState.Grabbing ||
            Enemy.state == EnemyState.Observing ||
            Enemy.isAVGFreeze)
            return;


        // 普通攻击请求
        if (!Enemy.IsHardLocked)
            Enemy.isAttack = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        if (Enemy.state == EnemyState.Grabbing) return; // 抓取中不改
        Enemy.isAttack = false;
    }
}
