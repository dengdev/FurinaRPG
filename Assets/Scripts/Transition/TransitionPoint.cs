using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionPoint : MonoBehaviour {
    public enum TransitionType { SameScene, DifferentScene }

    [Header("Transition Info")]
    public string destinationSceneName;
    public TransitionType transitionType;
    public TransitionDestination.DestinationTag destinationTag;
    private bool playerCanTransition;

    private void OnTriggerStay(Collider other) {
        if (other.CompareTag("Player")) {
            playerCanTransition = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            playerCanTransition = false;
        }
    }

    private void Update() {
        // �������Ƿ��ڴ��ͷ�Χ�ڲ����� "T" ����������
        if (playerCanTransition && Input.GetKeyDown(KeyCode.T)) {
            Debug.Log($"����T����׼�����͵�: {destinationTag}");
            SceneController.Instance.TransitionToDestination(this); // ���ó������������д���
            playerCanTransition = false; // ��ֹ�ظ�����
        }
    }
}
