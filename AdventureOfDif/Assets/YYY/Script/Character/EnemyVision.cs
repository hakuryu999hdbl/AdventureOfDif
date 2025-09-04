using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    public Enemy Enemy;

    //冲刺攻击重置
    public GameObject EnemyVision_2;


    private void OnTriggerStay2D(Collider2D collision)//检测到玩家显示
    {


        if (collision.gameObject.tag == "Player")
        {
            //暂时隐藏这个冲刺攻击功能
            if (Enemy.isChargeAttack == 2)
            { 
                Enemy.isChargeAttack = 0;
                Enemy.anim.Play("charge_hit");
                //EnemyVision_2.SetActive(true); 
            }//冲刺攻击
            else

            if (collision.gameObject.GetComponent<Player>().isDie && Enemy.isRape == false)
            {
                if (collision.gameObject.GetComponent<Player>().isRape == false)
                {
                  

                    // 寻找场景中所有其他敌人，设置围观
                    GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
                    foreach (GameObject e in allEnemies)
                    {
                        Enemy other = e.GetComponent<Enemy>();
                        if (other != null && other != Enemy && !Enemy.player.observingEnemies.Contains(other))
                        {
                            other.isRape = true; // 进入围观状态
                            Enemy.player.observingEnemies.Add(other);

                            Enemy.CleanupStatus();//清理状态
                        }
                    }

                    Enemy.CatchPlayer();
                }

            }
            else
            {
                Enemy.isAttack = true;
            }




            //if (collision.gameObject.GetComponent<Player>().isDie && collision.gameObject.GetComponent<Player>().isRape == false && Enemy.isRape == false)
            //{
            //
            //    Enemy.CatchPlayer();
            //}
            //else if (collision.gameObject.GetComponent<Player>().isDie && collision.gameObject.GetComponent<Player>().isRape && Enemy.isRape == false)
            //{
            //    //别的敌人抓住了玩家,站着不动
            //    Enemy.isRape = true;
            //
            //    // 加入围观列表（避免重复添加）
            //    if (!collision.gameObject.GetComponent<Player>().observingEnemies.Contains(Enemy))
            //        collision.gameObject.GetComponent<Player>().observingEnemies.Add(Enemy);
            //}
            //else
            //{
            //    Enemy.isAttack = true;
            //}

        }//敌人攻击玩家






    }

    private void OnTriggerExit2D(Collider2D collision)//检测到玩家显示
    {
        if (collision.gameObject.tag == "Player")
        {
            Enemy.isAttack = false;

        }//敌人停止攻击玩家


    }
}
