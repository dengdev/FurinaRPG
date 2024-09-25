using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : IPlayerState {
    private PlayerController player;

    public void Enter(PlayerController player) {
        this.player = player;
        player.animator.SetBool("IsMoving", true);
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            player.isRunning = !player.isRunning;
        }

        HandleMovement();

        if (Input.GetButtonDown("Jump")&& player.isGround) {
            player.ChangeState(new JumpState());
        }

        if (Input.GetMouseButtonDown(0)&& player.isGround) {
            player.ChangeState(new AttackState());
        }

        if (Input.GetAxis("Vertical") == 0 && Input.GetAxis("Horizontal") == 0) {
            player.ChangeState(new IdleState());
        }
    }

    public void Exit() {
        player.animator.SetBool("IsMoving", false);
    }

    private void HandleMovement() {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 desiredMoveDirection = GetDesiredMoveDirection(horizontal, vertical);
        UpdateMovement(desiredMoveDirection);
    }

    private Vector3 GetDesiredMoveDirection(float horizontal, float vertical) {
        Vector3 forward = player.cameraTransform.forward;
        Vector3 right = player.cameraTransform.right;
        forward.y = right.y = 0f;
        forward.Normalize();
        right.Normalize();
        return forward * vertical + right * horizontal;
    }

    private void UpdateMovement(Vector3 desiredMoveDirection) {
        if (desiredMoveDirection != Vector3.zero) {
            player.moveDirection = desiredMoveDirection;
            player.currentMoveSpeed = player.isRunning ? player.RunSpeed : player.WalkSpeed;
            player.animator.SetFloat("Speed", player.currentMoveSpeed); // �����ٶȶ�������
            player.transform.forward = player.moveDirection; // ��ɫ�泯�ƶ�����

            
        } else {
            player.moveDirection = Vector3.zero;
            player.currentMoveSpeed = 0f;
        }

        // Ӧ��ˮƽ�ƶ�
        Vector3 horizontalMovement = player.moveDirection * player.currentMoveSpeed * Time.deltaTime;
        player.characterController.Move(horizontalMovement);
    }

}
