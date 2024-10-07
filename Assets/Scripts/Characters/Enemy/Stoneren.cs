using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Rock;

public class Stoneren : EnemyController {
    [Header("ʯͷ�˼�������")]
    public float kickBackForce = 60;
    public float throwForce = 15;
    private Vector3 knockbackDirection;

    public Transform handPos;

    private void Start() {
    }
    private void Update() {
    }

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
                targetStats.TakeCharacterDamage(enemyData, targetStats);
            }
        }
    }

    // Animation Event
    public void Throwrock() {
        if (GameManager.Instance.rockPool == null) {
            GameManager.Instance.rockPool = new ObjectPool(ResourceManager.Instance.LoadResource<GameObject>("Prefabs/Rock")
, 3, 60, new GameObject("RockPool").transform);
        }
        // ��ʹ������뷶Χ��Ҳ�������һ��ʯͷ�����⶯��������
        GameObject rock = GameManager.Instance.rockPool.GetFromPool();
        if (rock != null) {

            rock.transform.position = handPos.position;
            Rigidbody rockRb = rock.GetComponent<Rigidbody>();

            Vector3 direction = (attackTarget.transform.position - transform.position).normalized;
            rockRb.AddForce(throwForce * direction, ForceMode.Impulse);
        }

    }
}
