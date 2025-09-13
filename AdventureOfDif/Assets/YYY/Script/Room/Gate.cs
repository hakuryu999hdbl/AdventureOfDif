using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{

    public Animator anim;

    private void OnTriggerStay2D(Collider2D collision)//检测到玩家显示
    {

        if (collision.gameObject.tag == "Player")
        {

            anim.SetTrigger("Open");
        }
        
    }
}
