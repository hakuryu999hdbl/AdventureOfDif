using UnityEngine;

public class Enemy_5 : EnemyController
{
    [Header("跳跃落地技能")]
    public bool canUseJumpStrike = true;

    [Tooltip("瞄准玩家的持续时间")]
    public float jumpAimTime = 1f;



    [Tooltip("瞬移落点和玩家之间的偏移")]
    public Vector2 landingOffset;

    [Tooltip("瞄准标记")]
    public Transform jumpAimTarget;


    [Tooltip("落地范围攻击碰撞体")]
    public GameObject jumpStrikeCollider;


    [HideInInspector]
    public Vector2 jumpStrikeTargetPos;

    public JumpStrikeSkillState jumpStrikeState =
        new JumpStrikeSkillState();


    public bool isJumpStrikeUntargetable;//跳跃攻击期间的无敌状态
    public override bool IgnoreIncomingDamage()
    {
        return isJumpStrikeUntargetable;
    }

    public override void EnterBattleState()
    {
        TryEnterJumpStrikeOrAttack();
    }

    public void TryEnterJumpStrikeOrAttack()
    {
        if (Random.value < 0.5f)
        {
            TransitionToState(jumpStrikeState);
        }
        else
        {
            TransitionToState(attackState);
        }

    }
}