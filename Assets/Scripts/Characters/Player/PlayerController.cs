using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    [Header("角色状态")]
    private float walkSpeed = 4f;
    private float runSpeed = 8f;
    public float playerJumpHeight = 2f;
    public float gravity = -9.8f; // 重力加速度
    public float currentMoveSpeed;
    public bool isGround;
    public bool playerIsAttacking; // 是否正在攻击

    public float jumpVelocity;
    public bool isRunning = false;
    public Vector3 moveDirection = Vector3.zero; // 角色移动方向


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
    public float knockbackDuration = 0.5f; // 击退时间
    private float knockbackTimer; // 击退的时间

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
            ChangeState(new DeathState()); // 切换到死亡状态
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
        moveDirection = Vector3.Lerp(moveDirection, Vector3.zero, Time.deltaTime * 5f); // 减缓击退效果
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
