using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour {
    [Header("Base Rock Setting")]
    [SerializeField] private int rockDamage = 10;
    [SerializeField] private float rockForce = 10.0f;
    [SerializeField] private GameObject breakEffect;
    [SerializeField] private int HitStoneRen_Multiply = 3;

    public enum RockStates { HitPlayer, HitEnemy, Default }
    public RockStates rockState;

    private Rigidbody rb;
    private float spawnTime = 0.5f;
    private float stateTimer;


    private void OnEnable() {
        StopAllCoroutines();
        rb = GetComponent<Rigidbody>();
        stateTimer = 0f;
        rockState = RockStates.HitPlayer;

        StartCoroutine(LifeCycle());
    }

    private void FixedUpdate() {
        if(rockState != RockStates.Default) {
            stateTimer += Time.fixedDeltaTime;
        }

        CheckRockState();
    }

    private void CheckRockState() {
        if (stateTimer > spawnTime && rb.velocity.sqrMagnitude < 1f) {
            rockState = RockStates.Default;
        }
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
            case RockStates.Default:
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
            }

            characterStats.TakeRockDamage(rockDamage, characterStats);
            rockState = RockStates.Default;
        }
    }

    private void HandleEnemyCollision(Collision collision) {
        if (collision.gameObject.GetComponent<Stoneren>()) {
            CharacterStats defender = collision.gameObject.GetComponent<CharacterStats>();
            defender.TakeRockDamage(rockDamage * HitStoneRen_Multiply, defender); 

            // 生成击碎特效
            Vector3 collisionPoint = collision.contacts[0].point;

            // 在碰撞点生成击碎特效
            Instantiate(breakEffect, collisionPoint, Quaternion.identity);

            // 将石块归还到对象池
            GameManager.Instance.rockPool.ReturnToPool(gameObject);
        }
    }

    private IEnumerator LifeCycle() {
        yield return new WaitForSeconds(10f);
        rb.GetComponent<Collider>().enabled=false;
        rb.isKinematic = true; 

        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + Vector3.down * 3f; 
        float moveDuration = 2f; 

        for (float t = 0; t < moveDuration; t += Time.deltaTime) {
            float normalizedTime = t / moveDuration;
            transform.position = Vector3.Lerp(startPosition, endPosition, normalizedTime);
            yield return null; 
        }
        transform.position = endPosition;
        rb.isKinematic = false;
        rb.GetComponent<Collider>().enabled = true;


        GameManager.Instance.rockPool.ReturnToPool(gameObject);
    }
}
