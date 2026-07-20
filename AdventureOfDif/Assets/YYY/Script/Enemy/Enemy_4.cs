using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_4 : EnemyController
{
    public override void EnterBattleState()
    {
        TransitionToState(attackState);
    }
}
