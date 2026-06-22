using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_1 : EnemyController
{
    public override void EnterBattleState()
    {
        //一半几率进入冲刺攻击一半几率进入普通攻击
        if (Random.value < 0.5f)
        {
            TransitionToState(chargeSkillState);
        }
        else
        {
            TransitionToState(attackState);
        }
    }
}
