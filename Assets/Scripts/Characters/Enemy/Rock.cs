using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour {
    [Header("Base Rock Setting")]
    public GameObject target;
    [SerializeField] private int rockDamage = 10;
    [SerializeField] private float rockForce = 10.0f;
    [SerializeField] private GameObject breakEffect;
    [SerializeField] private int HitStoneRen_Multiply = 3;

    public enum RockStates { HitPlayer, HitEnemy, HitNothing }
    public RockStates rockState;

    private Rigidbody rb;
    private float initialStateDuration = 0.5f;
    private float stateTimer;
    private bool isInInitialState = true; 

    private void Start() {
        rb = GetComponent<Rigidbody>();
        stateTimer = 0f;
        rockState = RockStates.HitPlayer;
        FlyToTarget();
    }

    private void FixedUpdate() {
        if (isInInitialState) {
            UpdateStateTimer();
        }
        CheckRockState();
    }

    private void UpdateStateTimer() {
        stateTimer += Time.fixedDeltaTime;
        if (stateTimer >= initialStateDuration) {
            isInInitialState = false;
        }
    }

    private void CheckRockState() {
        if (rb.velocity.sqrMagnitude < 1f) {
            rockState = RockStates.HitNothing;
        }
    }

    public void FlyToTarget() {
        if (target == null) {
            target = FindObjectOfType<PlayerController>().gameObject;
        }

       Vector3 direction = CalculateDirectionToTarget();
        rb.AddForce(rockForce * direction, ForceMode.Impulse); 
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
                break;
            default:
                break;
        }
    }
    private void HandlePlayerCollision(Collision collision) {

        if (collision.gameObject.CompareTag("Player")) {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            var characterStats = collision.gameObject.GetComponent<CharacterStats>();

            if (player != null) {
                Vector3 knockbackDirection = (collision.transform.position - transform.position).normalized;
                player.KnockbackPlayer(knockbackDirection * rockForce); // 调用Knockback方法
                Debug.Log("岩石击退玩家");
            }

            characterStats.TakeRockDamage(rockDamage, characterStats);
            rockState = RockStates.HitNothing;
        }
    }

    private void HandleEnemyCollision(Collision collision) {
        if (collision.gameObject.GetComponent<Stoneren>()) {
            CharacterStats defender = collision.gameObject.GetComponent<CharacterStats>();
            defender.TakeRockDamage(rockDamage * HitStoneRen_Multiply, defender); 
            Instantiate(breakEffect, transform.position, Quaternion.identity); // 生成击碎特效
            Destroy(gameObject);
        }
    }
}
