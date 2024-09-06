using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionPoint : MonoBehaviour
{
    /// <summary>
    /// 同或不同场景类型
    /// </summary>
    public enum TransionType { SameScene, DifferentScene }

    [Header("Transition Info")]
    public string sceneName;// 目标场景名
    public TransionType transionType; // 传送类型
    public TransitionDestination.DestionationTag destionationTag; // 目的地标记
    private bool canTrans;

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
            canTrans = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            canTrans = false;
    }

    private void Update()
    {
        // 检测按键T，如果玩家在碰撞体内并且按下了T
        if (canTrans && Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("可以传送");
           
        }
    }

}
