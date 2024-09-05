using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TransitionPoint : MonoBehaviour
{
    

    [Header("Transition Info")]
    public string sceneName;
    public TransionType transionType;

    public DestionationTag destionationTag;
    private bool canTrans;

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
            canTrans = true;
        Debug.Log(canTrans);

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            canTrans = false;
        Debug.Log(canTrans);

    }

}
