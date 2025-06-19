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
    
        //初始皮肤
        //ShowCurrentAll();
    
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
    #endregion

    /// <summary>
    /// 帧事件触发
    /// </summary>
    #region
    [Header("帧事件触发")]
    public Player player;
    public Enemy enemy;

    void Attack()
    {
        if (player != null)
        {
            if (player.isDie==false) { player.attack_Collider.SetActive(true); }//我方和敌方被击倒期间无法发出攻击碰撞体

        }
        if (enemy != null)
        {
            if (enemy.isDie == false) { enemy.attack_Collider.SetActive(true); }//我方和敌方被击倒期间无法发出攻击碰撞体

        }

        Invoke("HideAttack", 0.2f);
    }//攻击碰撞体闪出来一下就消失

    void HideAttack()
    {

        if (player != null)
        {
            player.attack_Collider.SetActive(false);
        }
        if (enemy != null)
        {
            enemy.attack_Collider.SetActive(false);
        }

    }//攻击碰撞体消失




    // attack_1 的中段帧事件
    public void EnableComboWindow()
    {
        if (player != null)
        {
            player.canCombo = true;

        }
       

    }




    //  attack_1 的结尾帧事件
    public void OnAttackAnimationEnd()
    {
        if (player != null)
        {
            player.canCombo = false;


            if (player.comboQueued && player.currentCombo < 4)
            {
                player.currentCombo++;
                player.anim.Play("attack_" + player.currentCombo, 0, 0);
                player.comboQueued = false;
            }
            else
            {
                player.ResetCombo();
            }

        }

        if (enemy != null)
        {
            enemy.anim.Play("stand");

        }
    }



    // grab_throw 投掷事件
    public void ThrowHeldObject() 
    {

        if (player != null)
        {
            player.ThrowHeldObject();

        }
    }

    #endregion


}
