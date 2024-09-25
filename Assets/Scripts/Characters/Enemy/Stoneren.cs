using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stoneren : EnemyController {
    [Header("ʯͷ�˼�������")]
    public float kickBackForce = 60;
    private Vector3 knockbackDirection; // ���˷���

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
                //Debug.Log("��ɫ��ѣ���һ���");
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
