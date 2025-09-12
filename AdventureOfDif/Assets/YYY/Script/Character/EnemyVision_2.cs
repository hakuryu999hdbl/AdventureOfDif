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
                Enemy.isChargeAttack = 1;
                Enemy.anim.Play("charge_ready");
                Invoke("Run", 1f);


                isTrigger = true;
                Invoke("SetTrigger", 2f);
            }
           
        }//敌人攻击玩家


    }

    private void Run()
    {
        Enemy.isChargeAttack = 2;
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
