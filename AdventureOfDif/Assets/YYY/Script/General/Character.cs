using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour, IDamageable
{
    [Header("数值")]
    public float maxHealth;
    public float currentHealth;
   

    [Header("受伤无敌")]
    public float invulnerableDuration;//无敌时长
    float invulnerableCounter;//计数器
    public bool invulnerable;//是否无敌

    [Header("受伤击退死亡")]
    public UnityEvent<Character> OnHealthChange;//只要生命值有一点改变，就把Character广播出去到ScriptObject
    public UnityEvent<Transform> OnTakeDamge;
    public UnityEvent OnDie;




    private void Start()
    {
        currentHealth = maxHealth;
     
        //传输Character过去
        OnHealthChange?.Invoke(this);

    }

    private void Update()
    {
        if (invulnerable)
        {
            invulnerableCounter -= Time.deltaTime;
            if (invulnerableCounter <= 0)
            {
                invulnerable = false;
            }
        }



      
    }


    public void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("DieZone"))
        {
            //死亡，更新血量
            currentHealth = 0;
            OnHealthChange?.Invoke(this);
            OnDie?.Invoke();
        }
    }

    public void TakeDamage(Attack attacker)
    {
        if (invulnerable) { return; }//处于无敌


        if (currentHealth - attacker.damage > 0)
        {
            currentHealth -= attacker.damage;
            TriggerInvulnerable();//打开无敌时间

            //受伤执行击退事件
            OnTakeDamge?.Invoke(attacker.transform);
            GetComponent<PlayerController>()?.OnTakeDamage(attacker); // 玩家直接击退
            GetComponent<EnemyController>()?.OnTakeDamage(attacker); // 敌人直接击退


        }
        else
        {
            currentHealth = 0;

            //触发死亡
            OnDie?.Invoke();
        }



        //传输Character过去
        OnHealthChange?.Invoke(this);

    }






    void TriggerInvulnerable()
    {
        if (!invulnerable)
        {
            invulnerable = true;
            invulnerableCounter = invulnerableDuration;
        }

    }//触发无敌

}
