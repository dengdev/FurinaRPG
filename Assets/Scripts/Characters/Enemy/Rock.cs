using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Rock : MonoBehaviour
{
    public enum RockStates { HitPlayer,HitEnemy,HitNothing}
    public RockStates rockState;
    private Rigidbody rb;

    [Header("Base Setting")]
    public float force;
    public GameObject target;
    public int damage;
    private Vector3 direction;
    public GameObject breakEffect;

   

    private void Start()
    {

        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.one;
        rockState = RockStates.HitPlayer;
        

        FlyToTarget();
    }

    private void FixedUpdate()
    {
        if(rb.velocity.sqrMagnitude<1f)
        {
            rockState= RockStates.HitNothing;
        }
    }

    public void FlyToTarget()
    {
        if (target == null)
        {
            target = FindObjectOfType<PlayerController>().gameObject;
        }
        direction = (target.transform.position - transform.position+Vector3.up).normalized;
        rb.AddForce(force * direction,ForceMode.Impulse);
    }


    private void OnCollisionEnter(Collision collision)
    {
        switch (rockState)
        {
            case RockStates.HitPlayer:
                if (collision.gameObject.CompareTag("Player"))
                {

                    // 玩家应该停止移动
                    collision.gameObject.GetComponent<NavMeshAgent>().isStopped = true;

                    collision.gameObject.GetComponent<NavMeshAgent>().velocity = direction * force;

                    collision.gameObject.GetComponent<Animator>().SetTrigger("Dizzy");

                    collision.gameObject.GetComponent<CharacterStats>().TakeDamage(damage, collision.gameObject.GetComponent<CharacterStats>());

                    rockState = RockStates.HitNothing;
                }
                break;
            case RockStates.HitEnemy:
                if (collision.gameObject.GetComponent<Stoneren>())
                {
                    collision.gameObject.GetComponent<CharacterStats>().TakeDamage(damage,collision.gameObject.GetComponent<CharacterStats>());
                    Instantiate(breakEffect,transform.position, Quaternion.identity);
                    Destroy(gameObject);
                }
                break;
            case RockStates.HitNothing:
                break;
            default:
                break;
        }
    }

}
