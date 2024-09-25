using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : IPlayerState {
    private PlayerController player;

    public void Enter(PlayerController player) {
        this.player = player;
        player.animator.SetTrigger("Jump");
        player.isGround = false;
        Jump();
    }

    public void Update() {
        player.jumpVelocity += player.gravity * Time.deltaTime;
        player.characterController.Move(player.moveDirection * player.currentMoveSpeed * Time.deltaTime + player.jumpVelocity * Time.deltaTime * Vector3.up);

        if (player.jumpVelocity < 0) {
            player.ChangeState(new IdleState()); // ÇÐ»»µ½ÏÐÖÃ×´Ì¬
        } 
    }

    public void Exit() {
        player.isGround = true;
    }

    private void Jump() {
        if (player.jumpVelocity < 0) {
            player.jumpVelocity = Mathf.Sqrt(2 * player.playerJumpHeight * -player.gravity);
        }
    }
}
