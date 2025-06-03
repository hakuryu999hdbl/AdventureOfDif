using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Strike : MonoBehaviour
{
    [Header("伤害对象")]
    public int Damage;
    public bool DamageToPlayer = true;
    public bool DamageToEnemy = true;
    public bool DamageToFriend = true;

    [Header("伤害类型")]
    public int TypeOfAttack;

    private int appliedDamage; // 当前生效的随机伤害
    private int baseDamage; // 原始设定伤害

    [Header("暴击")]
    public bool isCritial=false;
    public float chargeTime = 0f; // 由 Player 传入的蓄力时间
    private void OnEnable()
    {
        TypeOfAttack = 1;//剑伤

        baseDamage = Damage; // 保存原始值
        appliedDamage = baseDamage + Random.Range(-50, 50); // 例如±10范围

        if (isCritial)
        {
            appliedDamage *= 3; //暴击三倍伤害
        }
        else 
        {
            // 非暴击时，根据蓄力时间提升伤害：最大 1.5 倍（>=1秒）
            float chargeMultiplier = Mathf.Lerp(1f, 1.5f, Mathf.Clamp01(chargeTime));
            appliedDamage = Mathf.RoundToInt(appliedDamage * chargeMultiplier);
        }
        
    }//初始化随机伤害
    private void OnDisable()
    {
        appliedDamage = baseDamage; // 恢复初始值
        isCritial = false;
    }//隐藏时清除

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //敌人
        if (collision.gameObject.tag == "Player")
        {
            if (collision.gameObject.GetComponent<Player>() != null && DamageToPlayer)
            {


                collision.gameObject.GetComponent<Player>().ChangeHealth(appliedDamage, TypeOfAttack);//普通伤害


            }
        }




        //玩家和队友伤害
        if (collision.gameObject.tag == "Enemy")
        {
 
            if (collision.gameObject.GetComponent<Enemy>() != null && DamageToEnemy)
            {

                if (isCritial) { collision.gameObject.GetComponent<Enemy>().CritialAttack(); }//触发暴击（最先结算可以pass防御判断）

                collision.gameObject.GetComponent<Enemy>().ChangeHealth(appliedDamage, TypeOfAttack);//普通伤害

               

            }

            
        }




        if (collision.gameObject.tag == "Friend")
        {

            if (collision.gameObject.GetComponent<Enemy>() != null && DamageToFriend)
            {



                collision.gameObject.GetComponent<Enemy>().ChangeHealth(appliedDamage, TypeOfAttack);//普通伤害



            }


        }

    }

}
