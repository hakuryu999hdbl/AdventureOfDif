using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_2 : EnemyController
{
    [SerializeField, Min(0f)] private float minThrowDistance = 4f;

    public override void EnterBattleState()
    {
        Transform target = GetNearestTarget(attackList);
        PlayerController player = target != null ? target.GetComponent<PlayerController>() : null;
        TransitionToState(CanAimThrowAt(player) ? (EnemyBaseState)aimThrowSkillState : attackState);
    }

    public bool CanAimThrowAt(PlayerController player)
    {
        return player != null && !player.isDead && !player.isCaptured && !player.isHurt &&
            Vector2.Distance(transform.position, player.transform.position) >= minThrowDistance;
    }
}
