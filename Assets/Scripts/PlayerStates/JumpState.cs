using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class JumpState : IPlayerState {
    private PlayerController player;
    private readonly float gravity = -9.8f; // 重力加速度

    public void Enter(PlayerController player) {
        this.player = player;
        player.animator.SetTrigger("Jump");
        player.isGround = false;
        player.jumpVelocity = Mathf.Sqrt(2 * player.playerJumpHeight * -gravity);
    }

    public void Update() {
        player.jumpVelocity += gravity * Time.deltaTime;
        // 启用root，动画控制移动。
        //player.characterController.Move(player.jumpVelocity * Time.deltaTime * Vector3.up);//代码控制移动

        if (player.jumpVelocity<0) {
            player.ChangeState(new IdleState()); // 切换到闲置状态
        }
    }

    public void Exit() {
        player.isGround = true;
    }
}
