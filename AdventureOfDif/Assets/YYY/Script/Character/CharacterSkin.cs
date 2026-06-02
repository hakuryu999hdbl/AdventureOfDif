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




    public void HideSkeleton() 
    {
        skeletonAnimation.Skeleton.A = 0f; // 完全透明
    }

    public void ShowSkeleton()
    {
        skeletonAnimation.Skeleton.A = 1f; // 完全不透明
    }



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
            if (enemyController.isDead == false) { enemyController.attack_Collider_1.SetActive(true); }//我方和敌方被击倒期间无法发出攻击碰撞体

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
            if (enemyController.isDead == false) { enemyController.attack_Collider_2.SetActive(true); }//我方和敌方被击倒期间无法发出攻击碰撞体

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




    // attack_1 的中段帧事件
    public void EnableComboWindow()
    {
        if (playerController != null)
        {
            //playerController.canCombo = true;

        }
       

    }







    // grab_throw 投掷事件
    public void ThrowHeldObject() 
    {
    
        if (playerController != null)
        {
            //playerController.ThrowHeldObject();
    
        }
    
        if (enemyController != null)
        {
            //enemyController.ThrowHeldObject();
    
        }
    
    
    }

    //敌人死亡事件
    public void OnDie() 
    {
        if (enemyController != null)
        {
            Destroy(enemyController.gameObject);

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

    #endregion


}
