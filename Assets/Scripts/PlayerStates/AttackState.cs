using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : IPlayerState {
    private PlayerController player;
    private float ATTACK_TIME = 1.3f;
    private float hitRockForce = 25f;
    private float timer;

    private HashSet<Collider> hitTargets = new HashSet<Collider>();

    public void Enter(PlayerController player) {
        player.playerStats.isCritical = Random.value < player.playerStats.attackData.criticalChance;
        this.player = player;
        player.playerIsAttacking = true; // ���Ϊ���ڹ���
        player.animator.SetTrigger("Attack");
        timer = ATTACK_TIME;
        player.weapon.EnableWeaponCollider();
        hitTargets.Clear(); // ������ʼʱ��������е�Ŀ�꼯��
    }

    public void Update() {
        timer -=Time.deltaTime;

        Hit();
        if (timer<0) {
            player.ChangeState(new IdleState()); 
        }
    }

    public void Exit() {
        player.playerIsAttacking = false;
        player.weapon.DisableWeaponCollider();
    }


    private void Hit() {
        if (player.weapon.CheckCollision(out Collider target)) {
            if (hitTargets.Contains(target)) {
                return;
            }

            // �Ե���Ŀ����д���
            if (target.TryGetComponent<CharacterStats>(out CharacterStats targetStats)) {
                HandleEnemyAttack(targetStats);
                hitTargets.Add(target); // ��¼�ѻ��е�Ŀ��
            }

            // ����ʯĿ����д���
            if (target.TryGetComponent<Rock>(out Rock rockComponent)) {
                HandleRockAttack(rockComponent);
                hitTargets.Add(target); // ��¼�ѻ��е�Ŀ��
            }
        }
    }

    private void HandleRockAttack(Rock rock) {
        rock.rockState = Rock.RockStates.HitEnemy; 
        Rigidbody rb = rock.GetComponent<Rigidbody>();
        if (rb != null) {
            rb.velocity = Vector3.one; 
            rb.AddForce(player.transform.forward * hitRockForce, ForceMode.Impulse);
        }
    }

    private void HandleEnemyAttack(CharacterStats targetStats) {
        targetStats.TakeCharacterDamage(player.playerStats, targetStats);
    }
}
