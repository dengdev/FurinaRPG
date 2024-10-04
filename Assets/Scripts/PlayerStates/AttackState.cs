using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : IPlayerState {
    private PlayerController player;
    private const  float ATTACK_TIME = 1.3f;
    private const  float hitRockForce = 25f;
    private float timer;

    private HashSet<Collider> hitTargets = new HashSet<Collider>();

    private const float COLLIDER_ENABLE_TIME = 0.3f;
    private const float COLLIDER_DISABLE_TIME = 0.6f;
    private bool isColliderEnabled = false;


    public void Enter(PlayerController player) {
        this.player = player;
        player.playerStats.isCritical = Random.value < player.playerStats.attackData.criticalChance;
        player.playerIsAttacking = true; 
        player.animator.SetTrigger("Attack");
        timer = ATTACK_TIME;
        hitTargets.Clear();
        isColliderEnabled = false;
    }

    public void Update() {
        timer -=Time.deltaTime;

        AnimatorStateInfo stateInfo = player.animator.GetCurrentAnimatorStateInfo(0);

        if (!isColliderEnabled && stateInfo.normalizedTime >= COLLIDER_ENABLE_TIME && stateInfo.normalizedTime < COLLIDER_DISABLE_TIME) {
            player.weapon.EnableWeaponCollider();
            isColliderEnabled = true;
        }

        if (isColliderEnabled && stateInfo.normalizedTime >= COLLIDER_DISABLE_TIME) {
            player.weapon.DisableWeaponCollider();
            isColliderEnabled = false;
        }

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
        if (player.weapon.CheckCollision(out Collider target)&&!hitTargets.Contains(target)) {
            hitTargets.Add(target);

            if (target.TryGetComponent<CharacterStats>(out CharacterStats targetStats)) {
                HandleEnemyAttack(targetStats);
            } else if (target.TryGetComponent<Rock>(out Rock rockComponent)) {
                HandleRockAttack(rockComponent);
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
