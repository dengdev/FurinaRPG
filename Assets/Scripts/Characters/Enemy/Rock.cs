using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

/// <summary>
/// ��ʯͷ������һ��߷���ʯͷ��
/// </summary>
public class Rock : MonoBehaviour
{
    public enum RockStates { HitPlayer,HitEnemy,HitNothing}
    public RockStates rockState;
    private Rigidbody rb;

    [Header("Base Setting")]
    public GameObject target;
    [SerializeField] private int damage=10;// ʯͷ��ɵ��˺�
    [SerializeField] private float force=10.0f;
    [SerializeField] private GameObject breakEffect; // ������Ч
    [SerializeField] private Vector3 direction;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity= Vector3.one; // ʯͷ�ĳ�ʼ�ٶ�
        rockState = RockStates.HitPlayer; // ��ʼ״̬
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
        // ����������Ч��
        direction = (target.transform.position - transform.position+Vector3.up).normalized;
        rb.AddForce(force * direction,ForceMode.Impulse); // ��ʯͷһ����
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch (rockState)
        {
            case RockStates.HitPlayer:
                if (collision.gameObject.CompareTag("Player"))
                {
                    // ��ײ����ң�������ҵ�Knockback������������˷���
                    var playerController = collision.gameObject.GetComponent<PlayerController>();
                    var characterStats = collision.gameObject.GetComponent<CharacterStats>();
                    if (playerController != null)
                    {
                        Vector3 knockbackDirection = (collision.transform.position - transform.position).normalized; // ������˷���
                        playerController.Knockback(knockbackDirection * force); // ����Knockback����������˷��������
                    }
                    characterStats.TakeDamage(damage, characterStats);

                    rockState = RockStates.HitNothing;
                }
                break;
            case RockStates.HitEnemy:
                if (collision.gameObject.GetComponent<Stoneren>())
                {
                    var characterStats = collision.gameObject.GetComponent<CharacterStats>();
                    // ��ײ��ʯͷ�ˣ���������˺�
                    characterStats.TakeDamage(damage*3, characterStats);
                    // ����ʯͷ������Ч
                    Instantiate(breakEffect,transform.position, Quaternion.identity);
                    Destroy(gameObject);
                }
                break;
            case RockStates.HitNothing: // ���������û�з�����ײ
                break;
            default:
                break;
        }
    }
}
