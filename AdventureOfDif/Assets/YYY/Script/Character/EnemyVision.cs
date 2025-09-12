using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    public Enemy Enemy;




    private void OnTriggerStay2D(Collider2D collision)//检测到玩家显示
    {


        if (collision.gameObject.tag == "Player")
        {
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

                        }
                    }

                    Enemy.CatchPlayer();
                }

            }
            else
            {
                Enemy.isAttack = true;
            }




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
