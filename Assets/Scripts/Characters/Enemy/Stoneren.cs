using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stoneren : EnemyController {
    [Header("ʯͷ�˼�������")]
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
        // ��ʹ������뷶Χ��Ҳ�������һ��ʯͷ�����⶯��������
        var rock = Instantiate(rockPrefab, handPos.position, Quaternion.identity);
        rock.GetComponent<Rigidbody>().velocity = transform.forward;
        rock.GetComponent<Rock>().target = attackTarget;

    }
}
