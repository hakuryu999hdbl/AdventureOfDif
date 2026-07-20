using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using static GrabbableObject;

public class CharacterSkin : MonoBehaviour
{
    /// <summary>
    /// 皮肤
    /// </summary>
    #region
    //[Header("皮肤")]
    SkeletonMecanim skeletonAnimation;
    Skin blendSkin = new Skin("BlendedSkin");// 创建一个新的混合皮肤

    // Start is called before the first frame update
    void Start()
    {
        //换皮肤
        skeletonAnimation = GetComponent<SkeletonMecanim>();

        //淡入
        FadeIn(0.4f);
    }

    GrabbableObject.GrabbableType heldItemType;
    public void ShowCurrentAll(GrabbableObject.GrabbableType item)
    {
        //初始设置为混合皮肤

        switch (item) 
        {
            case GrabbableType.Tanker:
                blendSkin.AddSkin(skeletonAnimation.Skeleton.Data.FindSkin("color_1"));
                break;
            case GrabbableType.Inbox:
                blendSkin.AddSkin(skeletonAnimation.Skeleton.Data.FindSkin("color_2"));
                break;

        }


        skeletonAnimation.Skeleton.SetSkin(blendSkin);
        skeletonAnimation.Skeleton.SetSlotsToSetupPose();
    
        Debug.Log("设置皮肤");
    }




    #region  渐变进入 渐变消失

    public void FadeIn(float duration = 0.5f)
    {
        StopAllCoroutines();
        StartCoroutine(FadeInCoroutine(duration));
    }

    private IEnumerator FadeInCoroutine(float duration)
    {
        float timer = 0f;

        skeletonAnimation.Skeleton.A = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float alpha = Mathf.Lerp(
                0f,
                1f,
                timer / duration
            );

            skeletonAnimation.Skeleton.A = alpha;

            yield return null;
        }

        skeletonAnimation.Skeleton.A = 1f;
    }

    public void FadeOut(float duration = 0.5f)
    {
        StopAllCoroutines();
        StartCoroutine(FadeOutCoroutine(duration));
    }

    private IEnumerator FadeOutCoroutine(float duration)
    {
        float timer = 0f;

        skeletonAnimation.Skeleton.A = 1f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float alpha = Mathf.Lerp(
                1f,
                0f,
                timer / duration
            );

            skeletonAnimation.Skeleton.A = alpha;

            yield return null;
        }

        skeletonAnimation.Skeleton.A = 0f;
    }
    public void HideSkeleton()
    {
        skeletonAnimation.Skeleton.A = 0f; // 完全透明
    }

    public void ShowSkeleton()
    {
        skeletonAnimation.Skeleton.A = 1f; // 完全不透明
    }


    #endregion






    void Update()
    {

  
    }


    #endregion

    /// <summary>
    /// 帧事件触发
    /// </summary>
    #region
    [Header("帧事件触发")]
    public PlayerController playerController;
    public EnemyController enemyController;

    void Attack_1()
    {
        if (playerController != null)
        {
            if (playerController.isDead == false) { playerController.attack_Collider_1.SetActive(true); }//我方和敌方被击倒期间无法发出攻击碰撞体

        }
        if (enemyController != null)
        {
            if (enemyController.isDead == false &&!enemyController.isHurt &&!enemyController.isCatching) { enemyController.attack_Collider_1.SetActive(true); }//我方和敌方被击倒期间无法发出攻击碰撞体

        }

        Invoke("HideAttack", 0.2f);
    }//攻击碰撞体闪出来一下就消失

    void Attack_2()
    {
        if (playerController != null)
        {
            if (playerController.isDead == false) { playerController.attack_Collider_2.SetActive(true); }//我方和敌方被击倒期间无法发出攻击碰撞体

        }
        if (enemyController != null)
        {
            if (enemyController.isDead == false && !enemyController.isHurt && !enemyController.isCatching) { enemyController.attack_Collider_2.SetActive(true); }//我方和敌方被击倒期间无法发出攻击碰撞体

        }

        Invoke("HideAttack", 0.2f);
    }//攻击碰撞体闪出来一下就消失



    void HideAttack()
    {

        if (playerController != null)
        {
            playerController.attack_Collider_1.SetActive(false);
            playerController.attack_Collider_2.SetActive(false);
        }
        if (enemyController != null)
        {
            enemyController.attack_Collider_1.SetActive(false);
            enemyController.attack_Collider_2.SetActive(false);
        }

    }//攻击碰撞体消失












    // grab_throw 投掷事件
    public void ThrowHeldObject() 
    {
    
        if (playerController != null)
        {
            playerController.ThrowHeldObject();
    
        }
    
        if (enemyController != null)
        {
            enemyController.ThrowHeldObject();
    
        }
    
    
    }

    //敌人死亡事件
    public void OnDie() 
    {
        if (enemyController != null)
        {
            enemyController.DestroyEnemy();

        }
    }


    [Header("受伤结束控制")]
    public bool canAnimEndHurt = false;   // 动画帧事件是否允许结束受伤


    public void OnHurtOver()
    {
        if (playerController != null && canAnimEndHurt)
        {
            playerController.OnDamageOver();

        }



        if (enemyController != null&& canAnimEndHurt)
        {
            enemyController.OnDamageOver();

        }
    }



    //敌人冲刺技能结束
    public void OnChargeOver() 
    {
        if (enemyController != null)
        {
            enemyController.ChargeSkillOver();

        }
       
    }


    //敌人瞄准投掷
    public void AimThrowHeldObject()
    {


        if (enemyController != null)
        {
            enemyController.AimThrowSpawnExplosion();

        }


    }

    //敌人瞄准投掷技能结束
    public void OnThrowOver()
    {
        if (enemyController != null)
        {
            enemyController.AimThrowStartLaugh();

        }
    }

    //敌人嘲笑结束
    public void OnLaughOver() 
    {
        if (enemyController != null)
        {
            enemyController.AimThrowOver();

        }
    }


    //敌人将玩家投出去
    public void OnCatchPlayer() 
    {
        if (enemyController != null)
        {

            // 旧攻击动画的帧事件可能仍会执行，所以这里必须再次拦截
            if (enemyController.isDead ||
                enemyController.isHurt ||
                enemyController.isCatching)
            {
                enemyController.Catch_Collider.SetActive(false);
                return;
            }



            enemyController.Catch_Collider.SetActive(true);
        }

        CancelInvoke(nameof(HideCatch));//以防万一？

        Invoke("HideCatch", 0.2f);
    }
    void HideCatch()
    {
        if (enemyController != null)
        {
            enemyController.Catch_Collider.SetActive(false);
        }
    }//抓取碰撞体消失

    public void OnThrowCapturedPlayer() 
    {
        if (enemyController != null)
        {
            enemyController.ThrowCapturedPlayer();

        }
    }




    //敌人防御反击
    public void StartBlockCounter()
    {
        if (enemyController is Enemy_4 biker)
        {
            biker.StartCounterAttack();
        }
    }

    //敌人防御反击后
    public void EndBlockCounter()
    {
        if (enemyController is Enemy_4 biker)
        {
            biker.EndCounterAttack();
        }
    }


    #endregion


}
