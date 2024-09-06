using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionPoint : MonoBehaviour
{
    /// <summary>
    /// ͬ��ͬ��������
    /// </summary>
    public enum TransionType { SameScene, DifferentScene }

    [Header("Transition Info")]
    public string sceneName;// Ŀ�곡����
    public TransionType transionType; // ��������
    public TransitionDestination.DestionationTag destionationTag; // Ŀ�ĵر��
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
        // ��ⰴ��T������������ײ���ڲ��Ұ�����T
        if (canTrans && Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("���Դ���");
           
        }
    }

}
