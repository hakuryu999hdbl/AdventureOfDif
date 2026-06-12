using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_1 : EnemyController
{
    public override void EnterBattleState()
    {
        TransitionToState(chargeSkillState);
    }
}
