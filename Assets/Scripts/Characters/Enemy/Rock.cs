using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour {
    [Header("Base Setting")]
    public GameObject target;
    private int damage = 10;// 石头造成的伤害
    private float force = 10.0f;
    [SerializeField] private GameObject breakEffect; // 击碎特效
     private Vector3 direction;

    public enum RockStates { HitPlayer, HitEnemy, HitNothing }
    public RockStates rockState;

    private Rigidbody rb;

    private float initialStateDuration = 0.5f; // 初始状态持续时间
    private float stateTimer; // 计时器
    private bool isInInitialState = true; // 是否处于初始状态

    private void Start() {
        rb = GetComponent<Rigidbody>();
        stateTimer = 0f; // 初始化计时器
        rockState = RockStates.HitPlayer; // 初始状态
        FlyToTarget();
    }

    private void FixedUpdate() {
        if (isInInitialState) {
            UpdateStateTimer();
        }
        CheckRockState();
    }
    private void UpdateStateTimer() {
        stateTimer += Time.fixedDeltaTime; // 增加计时器
        if (stateTimer >= initialStateDuration) {
            isInInitialState = false; // 超过时间，停止更新计时器
        }
    }

    private void CheckRockState() {
        if (!isInInitialState) {
            if (rb.velocity.sqrMagnitude < 1f) {
                rockState = RockStates.HitNothing;
            }
        }
    }

    public void FlyToTarget() {
        if (target == null) {
            target = FindObjectOfType<PlayerController>().gameObject;
        }

        direction = CalculateDirectionToTarget();
        rb.AddForce(force * direction, ForceMode.Impulse); // 给石头一个力
    }

    private Vector3 CalculateDirectionToTarget() {
        // TODO:考虑更真实的物理轨迹
        return (target.transform.position - transform.position + Vector3.up).normalized;
    }

    private void OnCollisionEnter(Collision collision) {
        HandleCollision(collision);
    }

    private void HandleCollision(Collision collision) {
        switch (rockState) {
            case RockStates.HitPlayer:
                HandlePlayerCollision(collision);
                break;
            case RockStates.HitEnemy:
                HandleEnemyCollision(collision);
                break;
            case RockStates.HitNothing:
                // 什么都不做
                break;
            default:
                break;
        }
    }
    private void HandlePlayerCollision(Collision collision) {
        if (collision.gameObject.CompareTag("Player")) {
            var playerController = collision.gameObject.GetComponent<PlayerController>();
            var characterStats = collision.gameObject.GetComponent<CharacterStats>();

            if (playerController != null) {
                Vector3 knockbackDirection = CalculateKnockbackDirection(collision);
                playerController.KnockbackPlayer(knockbackDirection * force); // 调用Knockback方法
            }

            characterStats.TakeRockDamage(damage, characterStats);
            rockState = RockStates.HitNothing;
        }
    }

    private Vector3 CalculateKnockbackDirection(Collision collision) {
        return (collision.transform.position - transform.position).normalized; // 计算击退方向
    }

    private void HandleEnemyCollision(Collision collision) {
        if (collision.gameObject.GetComponent<Stoneren>()) {
            var characterStats = collision.gameObject.GetComponent<CharacterStats>();
            characterStats.TakeRockDamage(damage * 3, characterStats); // 碰撞到石头人，造成三倍伤害
            Instantiate(breakEffect, transform.position, Quaternion.identity); // 生成击碎特效
            Destroy(gameObject);
        }
    }
}
