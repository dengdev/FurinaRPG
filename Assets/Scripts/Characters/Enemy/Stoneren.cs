using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stoneren : EnemyController {
    [Header("石头人技能设置")]
    public float kickBackForce = 60;
    private Vector3 knockbackDirection; 

    public GameObject rockPrefab;
    public Transform handPos;

    // Animation Event
    public void KickOff() {
        if (attackTarget != null && transform.IsFacingTarget(attackTarget.transform)) {
            CharacterStats targetStats = attackTarget.GetComponent<CharacterStats>();

            if (attackTarget.GetComponent<PlayerController>() != null) {
                PlayerController player = attackTarget.GetComponent<PlayerController>();
                knockbackDirection = (player.transform.position - transform.position).normalized;
                player.KnockbackPlayer(knockbackDirection * kickBackForce);
                player.playerIsDizzy = true;
                player.ChangeState(new HitState());
                targetStats.TakeCharacterDamage(enemyStats, targetStats);
            }
        }
    }

    // Animation Event
    public void Throwrock() {
        // 即使玩家脱离范围，也生成最后一块石头，避免动画不流畅
        var rock = Instantiate(rockPrefab, handPos.position, Quaternion.identity);
        rock.GetComponent<Rigidbody>().velocity = transform.forward;
        rock.GetComponent<Rock>().target = attackTarget;

    }
}
