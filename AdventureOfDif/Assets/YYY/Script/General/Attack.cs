using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [Header("伤害数值")]
    public int damage;
    public int hitEffectType;//0打击 1斩击

    [Header("击飞参数")]
    public float knockbackX = 5f;
    public float knockbackY = 0f;
    public float hurtTime = 0.2f;
    public bool clearVelocity = true;


    private HashSet<IDamageable> hitTargets = new HashSet<IDamageable>();


    private bool comboAddedThisAttack;//如果是主角的Attack的话，算入连击
    public bool isPlayer = false;

    private void OnEnable()
    {
        hitTargets.Clear();

        comboAddedThisAttack = false;
    }

    private void OnTriggerStay2D(Collider2D other)
    {

       

        IDamageable target = other.GetComponentInParent<IDamageable>();

        if (target != null && !hitTargets.Contains(target))
        {

           
            hitTargets.Add(target);
            target.TakeDamage(this);

            // 本次攻击第一次有效命中
            if (isPlayer && !comboAddedThisAttack)
            {
                comboAddedThisAttack = true;

                RoomGenerator.instance.AddCombo();//连击显示
            }
        }

    }

}
