using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVision_2 : MonoBehaviour
{
    public Enemy Enemy;
    public bool isTrigger = false;

    private void OnTriggerEnter2D(Collider2D collision)//检测到玩家显示
    {

        if (collision.gameObject.tag == "Player"&&!isTrigger)
        {
            if (Enemy.IsGrounded()) 
            {
                isTrigger = true;//先别再触发


                //进入蓄力状态1秒
                Enemy.isChargeAttack = 1;
                Enemy.anim.Play("charge_ready");

                //跑步冲锋状态
                Invoke("Run", 1f);


                //重置状态
                //Invoke("SetTrigger", 2f);
            }
           
        }//敌人攻击玩家


    }

    private void Run()
    {
        Enemy.isChargeAttack = 2;

        //Enemy.CurrentTarget.transform.position = Enemy.transform.position;
    }

    private void SetTrigger()
    {
        if (Enemy.IsGrounded())
        {
            isTrigger = false;
        }
        else 
        {
            Invoke("SetTrigger", 2f);
        }
       
    }//如果在空中就别重置，再等2秒

}
