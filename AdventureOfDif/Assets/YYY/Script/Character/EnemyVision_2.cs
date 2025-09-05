using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVision_2 : MonoBehaviour
{
    public Enemy Enemy;
    public bool isTrigger = false;

    public int EnemyNumber;//1冲锋  2投掷

    private void OnTriggerEnter2D(Collider2D collision)//检测到玩家显示
    {

        if (collision.gameObject.tag == "Player"&&!isTrigger)
        {
            if (Enemy.IsGrounded()) 
            {
                isTrigger = true;//先别再触发

                switch (EnemyNumber) 
                {
                    case 1:
                        //进入蓄力状态1秒
                        Enemy.isChargeAttack = 1;
                        Enemy.anim.Play("charge_ready");

                        //跑步冲锋状态
                        Invoke("Run", 1f);
                        break;

                    case 2:
                        //进入蓄力状态1秒
                        Enemy.anim.Play("throw_ready");

                        //投掷状态
                        Invoke("Throw", 1f);
                        break;
                }

               


                
            }
           
        }//敌人攻击玩家


    }

    private void Run()
    {
        Enemy.isChargeAttack = 2;

        // 锁定目标点（生成一个空物体在玩家当前位置）
        GameObject lockPoint = new GameObject("ChargeTarget");
        lockPoint.transform.position = Enemy.player.transform.position;
        Enemy.LockTarget = lockPoint.transform;
    }

    private void Throw() 
    {
        Enemy.anim.Play("throw_out");
    }

    public void ResetChargeAttack() 
    {
        //重置状态
        Invoke("SetTrigger", 2f);
    }


    private void SetTrigger()
    {
        if (Enemy.IsGrounded())
        {
            isTrigger = false;

            Debug.Log("重置");
        }
        else 
        {
            Invoke("SetTrigger", 2f);
        }
       
    }//如果在空中就别重置，再等2秒

}
