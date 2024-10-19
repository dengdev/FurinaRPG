using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathState : IPlayerState {
    private CharacterController player;
    private float gameEndTime = 3.0f;

    public void Enter(PlayerController player) {
        player.animator.SetBool("Death", true); // ����������������
        GameManager.Instance.NotifyObservers(); // ��ɫ�������й㲥
       player.StartCoroutine(WaitAndShowGameOverScreen());
    }


    public void Update() {
    }

    public void Exit() {
    }

    private IEnumerator WaitAndShowGameOverScreen() {
        yield return new WaitForSeconds(gameEndTime);
        SceneController.Instance.ReturnMenuScene();
    }

}
