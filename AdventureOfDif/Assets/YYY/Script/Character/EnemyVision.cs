using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    public Enemy Enemy;
    public bool isFriend;

    public enum VisionType
    {
        ShortRangeVision,//负责拔刀和进入战斗状态
        LongRangeVision//负责选中目标敌人
    }

    [Header("敌人视觉类型")]
    public VisionType visionType = VisionType.ShortRangeVision;



    private void OnTriggerStay2D(Collider2D collision)//检测到玩家显示
    {

        if (!isFriend)
        {
            if (collision.gameObject.tag == "Player")
            {

              


                if (visionType == VisionType.ShortRangeVision) 
                {
                    Enemy.isAttack = true;

                }
               

                //Enemy.CurrentTarget = Enemy._Player.gameObject;
            }//敌人攻击玩家

            if (collision.gameObject.tag == "Friend")
            {


                if (visionType == VisionType.ShortRangeVision)
                {
                    Enemy.isAttack = true;

                }

                //Enemy.CurrentTarget = collision.gameObject;

            }//敌人攻击队友



        }
        else
        {
            if (collision.gameObject.tag == "Enemy")
            {


                if (visionType == VisionType.ShortRangeVision)
                {
                    Enemy.isAttack = true;

                }

                //Enemy.CurrentTarget = collision.gameObject;

            }//队友攻击敌人



        }

      
    }

    private void OnTriggerExit2D(Collider2D collision)//检测到玩家显示
    {
        if (!isFriend)
        {
            if (collision.gameObject.tag == "Player" )
            {
                if (visionType == VisionType.ShortRangeVision)
                {
                    Enemy.isAttack = false;
                }
            }//敌人停止攻击玩家

            if (collision.gameObject.tag == "Friend")
            {

                if (visionType == VisionType.ShortRangeVision)
                {
                    Enemy.isAttack = false;
                }

            }//敌人停止攻击队友

        }
        else
        {
            if (collision.gameObject.tag == "Enemy")
            {
                if (visionType == VisionType.ShortRangeVision)
                {
                    Enemy.isAttack = false;
                }

            }//队友停止攻击敌人



        }//队友停止射击敌人



    }
}
