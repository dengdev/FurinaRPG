using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shouren : EnemyController
{
    [Header("Skill")]
    public float kickBackForce = 20;
    private Vector3 knockbackDirection;

    public void KickOff()
    {
        if(attackTarget != null && transform.IsFacingTarget(attackTarget.transform))
        {
            transform.LookAt(attackTarget.transform);
            {
                knockbackDirection = (attackTarget.transform.position - transform.position).normalized;
                attackTarget.GetComponent<PlayerController>().Knockback(knockbackDirection* kickBackForce);

                attackTarget.GetComponent<Animator>().SetTrigger("Dizzy");
                Debug.Log("½ÇÉ«±»Ñ£ÔÎÇÒ»÷ÍË");

            }
        }
    }
}
