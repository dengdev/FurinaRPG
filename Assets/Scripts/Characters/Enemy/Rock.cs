using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

/// <summary>
/// 用石头攻击玩家或者反击石头人
/// </summary>
public class Rock : MonoBehaviour
{
    public enum RockStates { HitPlayer,HitEnemy,HitNothing}
    public RockStates rockState;
    private Rigidbody rb;

    [Header("Base Setting")]
    public GameObject target;
    [SerializeField] private int damage=10;// 石头造成的伤害
    [SerializeField] private float force=10.0f;
    [SerializeField] private GameObject breakEffect; // 击碎特效
    [SerializeField] private Vector3 direction;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity= Vector3.one; // 石头的初始速度
        rockState = RockStates.HitPlayer; // 初始状态
        FlyToTarget();
    }

    private void FixedUpdate()
    {
        if (rb.velocity.sqrMagnitude < 1f)
        {
            rockState = RockStates.HitNothing;
        }
    }

    public void FlyToTarget()
    {
        if (target == null)
        {
            target = FindObjectOfType<PlayerController>().gameObject;
        }
        // 增加抛物线效果
        direction = (target.transform.position - transform.position+Vector3.up).normalized;
        rb.AddForce(force * direction,ForceMode.Impulse); // 给石头一个力
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch (rockState)
        {
            case RockStates.HitPlayer:
                if (collision.gameObject.CompareTag("Player"))
                {
                    // 碰撞到玩家，调用玩家的Knockback函数，传入击退方向
                    var playerController = collision.gameObject.GetComponent<PlayerController>();
                    var characterStats = collision.gameObject.GetComponent<CharacterStats>();
                    if (playerController != null)
                    {
                        Vector3 knockbackDirection = (collision.transform.position - transform.position).normalized; // 计算击退方向
                        playerController.Knockback(knockbackDirection * force); // 调用Knockback方法传入击退方向和力度
                    }
                    characterStats.TakeDamage(damage, characterStats);

                    rockState = RockStates.HitNothing;
                }
                break;
            case RockStates.HitEnemy:
                if (collision.gameObject.GetComponent<Stoneren>())
                {
                    var characterStats = collision.gameObject.GetComponent<CharacterStats>();
                    // 碰撞到石头人，造成三倍伤害
                    characterStats.TakeDamage(damage*3, characterStats);
                    // 生成石头击碎特效
                    Instantiate(breakEffect,transform.position, Quaternion.identity);
                    Destroy(gameObject);
                }
                break;
            case RockStates.HitNothing: // 发射过程中没有发生碰撞
                break;
            default:
                break;
        }
    }
}
