using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shouren : EnemyController {
    [Header("Skill")]
    public float GrapForce = 20;
    private Vector3 GrapDirection;

    /// <summary>
    /// 动画中调用，后期要改
    /// </summary>
    public void Grap() {
        if (attackTarget != null && transform.IsFacingTarget(attackTarget.transform)) {
            var targetStats = attackTarget.GetComponent<CharacterStats>();

            if (attackTarget.GetComponent<PlayerController>() != null) {
                PlayerController player = attackTarget.GetComponent<PlayerController>();
                GrapDirection = (transform.position - player.transform.position).normalized;
                player.KnockbackPlayer(GrapDirection * GrapForce);
                player.playerIsDizzy = true;
                player.ChangeState(new HitState());
                targetStats.TakeCharacterDamage(enemyStats, targetStats);
            }
        }
    }
}
