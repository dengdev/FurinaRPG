using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class JumpState : IPlayerState {
    private PlayerController player;
    private readonly float gravity = -9.8f; // �������ٶ�

    public void Enter(PlayerController player) {
        this.player = player;
        player.animator.SetTrigger("Jump");
        player.isGround = false;
        player.jumpVelocity = Mathf.Sqrt(2 * player.playerJumpHeight * -gravity);
    }

    public void Update() {
        player.jumpVelocity += gravity * Time.deltaTime;
        // ����root�����������ƶ���
        //player.characterController.Move(player.jumpVelocity * Time.deltaTime * Vector3.up);//��������ƶ�

        if (player.jumpVelocity<0) {
            player.ChangeState(new IdleState()); // �л�������״̬
        }
    }

    public void Exit() {
        player.isGround = true;
    }
}
