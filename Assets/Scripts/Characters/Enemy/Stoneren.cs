using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stoneren : EnemyController
{
    [Header("Skill")]
    public float kickBackForce = 60;
    private Vector3 knockbackDirection;

    public GameObject rockPrefab;
    public Transform handPos;

    /// <summary>
    /// Animation Event
    /// </summary>
    public void KickOff()
    {
        if (attackTarget != null && transform.IsFacingTarget(attackTarget.transform))
        {
            transform.LookAt(attackTarget.transform);
            {
                knockbackDirection = (attackTarget.transform.position - transform.position).normalized;
                attackTarget.GetComponent<PlayerController>().Knockback(knockbackDirection * kickBackForce);

                attackTarget.GetComponent<Animator>().SetTrigger("Dizzy");
                Debug.Log("½ÇÉ«±»Ñ£ÔÎÇÒ»÷ÍË");

            }
        }
    }

    /// <summary>
    /// Animation Event
    /// </summary>
    public void Throwrock()
    {
        if (attackTarget != null)
        {
            var rock=Instantiate(rockPrefab,handPos.position,Quaternion.identity);
            rock.GetComponent<Rock>().target= attackTarget;
        }
    }
}
