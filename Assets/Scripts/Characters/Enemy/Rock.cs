using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour {
    [Header("Base Setting")]
    public GameObject target;
    private int damage = 10;// ʯͷ��ɵ��˺�
    private float force = 10.0f;
    [SerializeField] private GameObject breakEffect; // ������Ч
     private Vector3 direction;

    public enum RockStates { HitPlayer, HitEnemy, HitNothing }
    public RockStates rockState;

    private Rigidbody rb;

    private float initialStateDuration = 0.5f; // ��ʼ״̬����ʱ��
    private float stateTimer; // ��ʱ��
    private bool isInInitialState = true; // �Ƿ��ڳ�ʼ״̬

    private void Start() {
        rb = GetComponent<Rigidbody>();
        stateTimer = 0f; // ��ʼ����ʱ��
        rockState = RockStates.HitPlayer; // ��ʼ״̬
        FlyToTarget();
    }

    private void FixedUpdate() {
        if (isInInitialState) {
            UpdateStateTimer();
        }
        CheckRockState();
    }
    private void UpdateStateTimer() {
        stateTimer += Time.fixedDeltaTime; // ���Ӽ�ʱ��
        if (stateTimer >= initialStateDuration) {
            isInInitialState = false; // ����ʱ�䣬ֹͣ���¼�ʱ��
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
        rb.AddForce(force * direction, ForceMode.Impulse); // ��ʯͷһ����
    }

    private Vector3 CalculateDirectionToTarget() {
        // TODO:���Ǹ���ʵ������켣
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
                // ʲô������
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
                playerController.KnockbackPlayer(knockbackDirection * force); // ����Knockback����
            }

            characterStats.TakeRockDamage(damage, characterStats);
            rockState = RockStates.HitNothing;
        }
    }

    private Vector3 CalculateKnockbackDirection(Collision collision) {
        return (collision.transform.position - transform.position).normalized; // ������˷���
    }

    private void HandleEnemyCollision(Collision collision) {
        if (collision.gameObject.GetComponent<Stoneren>()) {
            var characterStats = collision.gameObject.GetComponent<CharacterStats>();
            characterStats.TakeRockDamage(damage * 3, characterStats); // ��ײ��ʯͷ�ˣ���������˺�
            Instantiate(breakEffect, transform.position, Quaternion.identity); // ���ɻ�����Ч
            Destroy(gameObject);
        }
    }
}
