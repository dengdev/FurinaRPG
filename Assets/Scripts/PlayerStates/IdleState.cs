using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IPlayerState {
    private PlayerController player;

    public void Enter(PlayerController player) {
        this.player = player;
        player.animator.SetBool("IsIdle", true);
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            player.isRunning = !player.isRunning;
        }

        if (Input.GetButtonDown("Jump") && player.isGround) {
            player.ChangeState(new JumpState());
        }

        if (Input.GetMouseButtonDown(0) && player.isGround) {
            player.ChangeState(new AttackState());
        }

        if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0) {
            player.ChangeState(new MoveState());
        }
    }

    public void Exit() {
        player.animator.SetBool("IsIdle", false);
    }
}
