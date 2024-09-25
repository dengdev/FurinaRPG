using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    [Header("��ɫ״̬")]
    private float walkSpeed = 4f;
    private float runSpeed = 8f;
    public float playerJumpHeight = 2f;
    public float gravity = -9.8f; // �������ٶ�
    public float currentMoveSpeed;
    public bool isGround;
    public bool playerIsAttacking; // �Ƿ����ڹ���

    public float jumpVelocity;
    public bool isRunning = false;
    public Vector3 moveDirection = Vector3.zero; // ��ɫ�ƶ�����


    public float WalkSpeed {
        get { return walkSpeed; }
        set { walkSpeed = value; }
    }

    public float RunSpeed {
        get { return runSpeed; }
        set { runSpeed = value; }
    }

    [Header("Attack Settings")]
    public Weapon weapon;
    public float playerAttackRange;
    public float knockbackDuration = 0.5f; // ����ʱ��
    private float knockbackTimer; // ���˵�ʱ��

    public  Transform cameraTransform;
    public CharacterController characterController;
    public Animator animator;
    public CharacterStats playerStats;
    private IPlayerState currentState;

    public CharacterStats GetplayerStats { get { return playerStats; } }


    private void Awake() {
        animator = GetComponent<Animator>();
        playerStats = GetComponent<CharacterStats>();
        cameraTransform = Camera.main.transform;
        characterController = GetComponent<CharacterController>();
        GameManager.Instance.RegisterPlayer(playerStats);
    }

    private void OnEnable() {
        GameManager.Instance.RegisterPlayer(playerStats);
    }

    void Start() {
        isGround=true;
        SaveManager.Instance.LoadPlayerData();
        ChangeState(new IdleState());
    }

    void Update() {
        currentState?.Update();

        if (playerStats.CurrentHealth <= 0 && currentState is not DeathState) {
            ChangeState(new DeathState()); // �л�������״̬
            return;
        }

        if (knockbackTimer > 0) {
            HandleKnockback();
        }
    }

    public void ChangeState(IPlayerState newState) {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter(this);
    }

    private void HandleKnockback() {
        knockbackTimer -= Time.deltaTime;
        characterController.Move(moveDirection * Time.deltaTime);
        moveDirection = Vector3.Lerp(moveDirection, Vector3.zero, Time.deltaTime * 5f); // ��������Ч��
    }


    public void KnockbackPlayer(Vector3 knockbackForce) {
        knockbackTimer = knockbackDuration;
        moveDirection = knockbackForce;
    }



    private void OnDrawGizmos() {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, playerAttackRange);
    }
}
