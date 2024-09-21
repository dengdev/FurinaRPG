using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shouren : EnemyController
{
    [Header("Skill")]
    public float kickBackForce = 20;
    private Vector3 knockbackDirection;

    /// <summary>
    /// �����е��ã�����Ҫ��
    /// </summary>
    public void KickOff()
    {
        if(attackTarget != null && transform.IsFacingTarget(attackTarget.transform))
        {
            {
                knockbackDirection = (attackTarget.transform.position - transform.position).normalized;
                attackTarget.GetComponent<PlayerController>().KnockbackPlayer(knockbackDirection* kickBackForce);

                attackTarget.GetComponent<Animator>().SetTrigger("Dizzy");
                Debug.Log("��ɫ��ѣ���һ���");

            }
        }
    }
}
