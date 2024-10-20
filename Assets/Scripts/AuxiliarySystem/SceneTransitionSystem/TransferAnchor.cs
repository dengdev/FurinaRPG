using System.Collections;
using UnityEngine;
using static UnityEngine.ProBuilder.AutoUnwrapSettings;

public class TransferAnchor : MonoBehaviour {

    [Header("传送锚点设置")]
    public string crossScene;
    public TransferAnchorTag anchorID;
    public TransferAnchorTag destinationTag;
    [SerializeField]private SceneTransitionType transitionType;


    private bool isPlayerInTransitionZone;

    private void Start() {
        if (!IsSameScene()&&crossScene==null) {
            Debug.LogWarning(this.name + "当前传送门是不同场景传送，但未赋值");
        }
    }

    private void OnTriggerStay(Collider other) {
        if (other.CompareTag("Player")) {
            isPlayerInTransitionZone = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            isPlayerInTransitionZone = false;
        }
    }

    private void Update() {
        if (isPlayerInTransitionZone && Input.GetKeyDown(KeyCode.T)) {
            SceneManager.Instance.TransferToTargetAnchor(this); 
            isPlayerInTransitionZone = false; 
        }
    }

    public bool IsSameScene() {
        return this.transitionType == SceneTransitionType.SameScene;
    }
}
