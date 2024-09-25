using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stoneren : EnemyController {
    [Header("石头人技能设置")]
    public float kickBackForce = 60;
    private Vector3 knockbackDirection; // 击退方向

    public GameObject rockPrefab;
    public Transform handPos;

    // Animation Event
    public void KickOff() {
        if (attackTarget != null && transform.IsFacingTarget(attackTarget.transform)) {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            {
                knockbackDirection = (attackTarget.transform.position - transform.position).normalized;
                attackTarget.GetComponent<PlayerController>().KnockbackPlayer(knockbackDirection * kickBackForce);

                //attackTarget.GetComponent<Animator>().SetTrigger("Dizzy");
                //Debug.Log("角色被眩晕且击退");
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
