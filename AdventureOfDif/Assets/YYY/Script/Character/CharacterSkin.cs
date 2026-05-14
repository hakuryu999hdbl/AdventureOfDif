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

       // if (player != null)
       // {
       //     // 根据玩家是否在地面上调整层级
       //     int targetOrder = player.IsGrounded() ? 0 : 1;
       //
       //     skeletonAnimation.GetComponent<MeshRenderer>().sortingOrder = targetOrder;
       // }
       // if (enemy != null)
       // {
       //     // 根据玩家是否在地面上调整层级
       //     int targetOrder = enemy.IsGrounded() ? 0 : 1;
       //
       //     skeletonAnimation.GetComponent<MeshRenderer>().sortingOrder = targetOrder;
       // }
    }


    #endregion

    /// <summary>
    /// 帧事件触发
    /// </summary>
    #region
    [Header("帧事件触发")]
    public PlayerController playerController;
    public EnemyController enemyController;

    void Attack()
    {
        if (playerController != null)
        {
            if (playerController.isDead == false) { playerController.attack_Collider.SetActive(true); }//我方和敌方被击倒期间无法发出攻击碰撞体

        }
        if (enemyController != null)
        {
            if (enemyController.isDead == false) { enemyController.attack_Collider.SetActive(true); }//我方和敌方被击倒期间无法发出攻击碰撞体

        }

        Invoke("HideAttack", 0.2f);
    }//攻击碰撞体闪出来一下就消失

    void HideAttack()
    {

        if (playerController != null)
        {
            playerController.attack_Collider.SetActive(false);
        }
        if (enemyController != null)
        {
            enemyController.attack_Collider.SetActive(false);
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




    #endregion


}
