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


        //已经成功抓住玩家后，直到投出前暂时无敌
        EnemyController enemy = GetComponent<EnemyController>();

        if (enemy != null && enemy.isCatching)
        {
            return;
        }


        // 摩托哥防御反击
        if (enemy != null && enemy.TryHandleIncomingAttack(attacker))
        {
            Debug.Log("攻击已被特殊状态处理");
            return;
        }

        // Enemy_5 悬空期间无敌
        if (enemy != null && enemy.IgnoreIncomingDamage())
            return;

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



    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);

        //传输Character过去
        OnHealthChange?.Invoke(this);

        GetComponent<PlayerController>()?.GreenScreen.SetActive(true); // 玩家回血绿屏幕


    }//非Attack系的直接回血端口

    public void SaveTempState()
    {
        GameFlowData.playerHealth = currentHealth;
        //GameFlowData.playerSex = currentSex;
    }//Dif这个项目需要跨场景记录数值
    public void LoadTempState()
    {
        if (!GameFlowData.HasPlayerState)
        {
            // 没有临时记录，说明从主菜单开始新游戏
            currentHealth = maxHealth;

            // 根据你的设计决定初始值
            //currentSex = 0;

            Debug.Log("没有临时状态，使用新游戏初始数值");
        }
        else
        {
            currentHealth = Mathf.Clamp(
                GameFlowData.playerHealth.Value,
                0,
                maxHealth
            );

            //if (GameFlowData.playerSex.HasValue)
            //{
            //    currentSex = Mathf.Clamp(
            //        GameFlowData.playerSex.Value,
            //        0,
            //        maxSex
            //    );
            //}
            //
            //Debug.Log(
            //    $"恢复临时状态：HP={currentHealth}，Sex={currentSex}"
            //);
        }

        //传输Character过去
        OnHealthChange?.Invoke(this);
    }//初始跨场景读取数值





    void TriggerInvulnerable()
    {
        if (!invulnerable)
        {
            invulnerable = true;
            invulnerableCounter = invulnerableDuration;
        }

    }//触发无敌

}
