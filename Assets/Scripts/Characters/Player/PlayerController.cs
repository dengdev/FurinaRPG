using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    [Header("角色状态")]
    private float walkSpeed = 4f;
    private float runSpeed = 8f;
    public float playerJumpHeight = 6f;
    public float currentMoveSpeed;
    public bool isGround;
    public bool playerIsAttacking;
    public bool playerIsHIt;
    public bool playerIsDizzy;

    public float jumpVelocity;
    public bool isRunning = false;
    public Vector3 moveDirection = Vector3.zero;

    public float WalkSpeed {
        get { return walkSpeed; }
        set { walkSpeed = value; }
    }

    public float RunSpeed {
        get { return runSpeed; }
        set { runSpeed = value; }
    }

    [Header("Attack Settings")]
    public WeaponAttack weapon;
    public float knockbackDuration = 0.5f;
    private float knockbackTimer;

    public Transform cameraTransform;
    public CharacterController characterController;
    public Animator animator;
    public CharacterStats controllerPlayerStats;
    private IPlayerState currentState;


    private void Awake() {
        animator = GetComponent<Animator>();
        controllerPlayerStats = GetComponent<CharacterStats>();
        cameraTransform = Camera.main.transform;
        characterController = GetComponent<CharacterController>();
    }

    private void OnEnable() {
        Debug.Log("玩家控制器试图注册");
        GameManager.Instance.RegisterPlayer(controllerPlayerStats);
    }

    private void Start() {
        Debug.Log("玩家控制器加载玩家数据,并且试图再次注册");
        SaveManager.Instance.Load();
        GameManager.Instance.RegisterPlayer(controllerPlayerStats);

        if (GameManager.Instance.playerData == null) {
            GameManager.Instance.playerData = new PlayerData(
                    currentLevel: 1,
                    maxLevel: 20,
                    baseExp: 80,
                    currentExp: 0,
                    levelBuff: 0.1f,
                    maxHealth: 100,
                    currentHealth: 100,
                    baseDefence: 2,
                    currentDefence: 2
                    );
            Debug.Log("游戏管理员的玩家统计没有数据，于是初始化数据");
        }
        controllerPlayerStats.characterData = GameManager.Instance.playerData;

        isGround = true;
        ChangeState(new IdleState());
    }


    void Update() {
        if (Time.timeScale == 0) return;
        currentState?.Update();

        if (controllerPlayerStats.CurrentHealth <= 0 && currentState is not DeathState) {
            ChangeState(new DeathState()); // 切换到死亡状态
            return;
        }

        if (Input.GetKeyDown(KeyCode.F)) {
            Debug.Log("按下了F键");
            GameManager.Instance.playerData.items.Add(SaveManager.Instance.allItems[1]);
            GameManager.Instance.playerData.items.Add(SaveManager.Instance.allItems[2]);
        }

        if (knockbackTimer > 0) {
            HandleKnockback();
        }
    }

    public bool NowIsHitState() {
        return currentState is HitState;
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
}
