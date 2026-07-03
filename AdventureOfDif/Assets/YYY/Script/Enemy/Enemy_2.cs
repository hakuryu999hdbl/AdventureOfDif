using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_2 : EnemyController
{
    public override void EnterBattleState()
    {
        //一半几率进入瞄准投掷攻击一半几率进入普通攻击
        //if (Random.value < 0.5f)
        //{
        //    TransitionToState(aimThrowSkillState);
        //}
        //else
        //{
        //    TransitionToState(attackState);
        //}

        TransitionToState(aimThrowSkillState);
    }
}
