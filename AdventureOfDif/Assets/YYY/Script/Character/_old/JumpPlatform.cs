using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPlatform : MonoBehaviour
{
    public float platformZHeight = 3f; // 进平台时设置的地面基准（例：3）
    public float exitZHeight = 0.01f;  // 离开平台时恢复的地面基准（例：0.01 ≈ 0）

    public GameObject NoWalkOn;
    private void OnTriggerStay2D(Collider2D collision)//检测到玩家显示
    {


        if (collision.gameObject.tag == "Player")
        {

            Debug.Log("碰到玩家");

            if (collision.gameObject.GetComponent<Player>()!= null)
            {

                var player = collision.GetComponent<Player>();
                if (player != null)
                {

                    player.Platform = platformZHeight;



                    Debug.Log("触发平台高度设置：" + transform.name);

                    NoWalkOn.SetActive(false);
                }

            }


        }






    }

    private void OnTriggerExit2D(Collider2D collision)//检测到玩家显示
    {
       

        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("离开玩家");

            if (collision.gameObject.GetComponent<Player>() != null)
            {

                var player = collision.GetComponent<Player>();
                if (player != null)
                {


                    player.Platform = exitZHeight;
                    Debug.Log("离开平台：" + transform.name);

                    NoWalkOn.SetActive(true);


                }

            }

        }


    }
}
