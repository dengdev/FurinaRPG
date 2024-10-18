using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stoneren : EnemyController {
    [Header("石头人技能设置")]
    public float kickBackForce = 60;
    public float throwForce = 15;
    private Vector3 knockbackDirection;

    public Transform handPos;


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

    public override void PerformEnemyAttack() {
        enemyData.isCritical = Random.value < enemyData.attackData.criticalChance;
        transform.LookAt(attackTarget.transform);

        if (WithinAttackRange()) {
            animator.SetTrigger("Attack");
            StartCoroutine(AttackDelayCoroutine(1f)); 
        }

        if (WithinSkillRange()) {
            animator.SetTrigger("Skill");
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("ThrowRock")) {
                if (stateInfo.normalizedTime == 0.6f) {
                    Throwrock();
                }

            }

        }
    }

    private IEnumerator AttackDelayCoroutine(float delay) {
        // 等待指定的时间
        yield return new WaitForSeconds(delay);
        KickOff();
    }


    public void Throwrock() {

        if (GameManager.Instance.rockPool == null) {
            GameManager.Instance.rockPool = new ObjectPool(ResourceManager.Instance.LoadResource<GameObject>("Prefabs/Enemy/Rock"), 3, 60, new GameObject("RockPool").transform);
        }

        GameObject rock = GameManager.Instance.rockPool.GetFromPool();
        rock.transform.position = handPos.position;

        Vector3 direction;
        if (attackTarget != null) {
            direction = attackTarget.transform.position - transform.position;
        } else {
            direction = lastTargetPosition - transform.position;
        }
        rock.GetComponent<Rigidbody>().AddForce(throwForce * (direction.normalized), ForceMode.Impulse);
    }
}
