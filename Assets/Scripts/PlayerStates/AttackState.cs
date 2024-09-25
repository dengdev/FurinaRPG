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
        player.playerIsAttacking = true; // 标记为正在攻击
        player.animator.SetTrigger("Attack");
        timer = ATTACK_TIME;
        player.weapon.EnableWeaponCollider();
        hitTargets.Clear(); // 攻击开始时清空已命中的目标集合
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

            // 对敌人目标进行处理
            if (target.TryGetComponent<CharacterStats>(out CharacterStats targetStats)) {
                HandleEnemyAttack(targetStats);
                hitTargets.Add(target); // 记录已击中的目标
            }

            // 对岩石目标进行处理
            if (target.TryGetComponent<Rock>(out Rock rockComponent)) {
                HandleRockAttack(rockComponent);
                hitTargets.Add(target); // 记录已击中的目标
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
