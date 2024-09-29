using System.Collections;
using UnityEngine;

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
        // 检测玩家是否在传送范围内并按下 "T" 键触发传送
        if (playerCanTransition && Input.GetKeyDown(KeyCode.T)) {
            Debug.Log($"按下T键，准备传送到: {destinationTag}");
            SceneController.Instance.TransitionToDestination(this); // 调用场景控制器进行传送
            playerCanTransition = false; // 防止重复传送
        }
    }
}
