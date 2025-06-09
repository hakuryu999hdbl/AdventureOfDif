using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public Enemy Enemy;//获取链接RoomGenerator能力

    private void Start()
    {
        Invoke("ChangeTargetPlace", 1f);//一开始就把Target摆到场景里

    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        //进入动画
        if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Friend")//敌人与NPC都能触发
        {
            ChangeTargetPlace();

            //Debug.Log("到位置了换地方");
        }
    }



    private void ChangeTargetPlace()
    {
        //Enemy.RoomGenerator.ChangeTargetPlace(this.gameObject,3);
    }
}
