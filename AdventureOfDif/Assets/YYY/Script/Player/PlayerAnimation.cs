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
    }

    public void PlayHurt()
    {
        anim.SetInteger("hurtType", Random.Range(1, 3));
        anim.SetTrigger("hurt");
    }

    public void PlayAttack()
    {
        anim.SetTrigger("attack");
    }

}
