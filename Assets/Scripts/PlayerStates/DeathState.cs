using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathState : IPlayerState {
    private CharacterController player;
    private float gameEndTime = 3.0f;

    public void Enter(PlayerController player) {
        player.animator.SetBool("Death", true); // 更新死亡动画参数
        GameManager.Instance.NotifyObservers(); // 角色死亡进行广播
       player.StartCoroutine(WaitAndShowGameOverScreen());
    }


    public void Update() {
    }

    public void Exit() {
    }

    private IEnumerator WaitAndShowGameOverScreen() {
        yield return new WaitForSeconds(gameEndTime);
        SceneController.Instance.TransitionToMainMenuScene();
    }

}
