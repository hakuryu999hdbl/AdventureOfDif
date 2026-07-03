using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [Header("联系玩家脚本")]
    public Animator anim;
    public PlayerController playerController;
    float moveSpeedValue;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 velocity = playerController.rb.velocity;
        moveSpeedValue = velocity.magnitude;

        anim.SetFloat("Speed", moveSpeedValue);
        anim.SetBool("isDead", playerController.isDead);
        anim.SetBool("isAttack", playerController.isAttack);
        anim.SetBool("isDashAttack", playerController.isDashAttack);
        anim.SetBool("IsGrabbing", playerController.isGrabbing);
    }

    public void PlayHurt()
    {
        anim.SetBool("hurt", true);
        anim.SetInteger("hurtType", Random.Range(1, 3));
    }
    public void PlayFly()
    {
        anim.SetBool("hurt", true);
        anim.SetInteger("hurtType", 3);
    }
    public void EndHurt()
    {
        anim.SetBool("hurt", false);
        anim.SetBool("down", false);
        anim.SetInteger("hurtType", 0);
    }


    public void PlayAttack()
    {
        if (playerController.isGrabbing)
        {
            anim.SetTrigger("throw");
        }
        else 
        {
            anim.SetTrigger("attack");
        }

        
    }

}
