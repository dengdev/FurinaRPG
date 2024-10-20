using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    [Header("½ÇÉ«×´Ì¬")]
    private float walkSpeed = 4f;
    private float runSpeed = 8f;
    public float playerJumpHeight = 6f;
    public float currentMoveSpeed;
    public bool isGround;
    public bool playerIsAttacking;
    public bool playerIsHIt;
    public bool playerIsDizzy;
    public Queue<float> hitTimestamps = new Queue<float>();


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
        GameManager.Instance.RegisterPlayer (controllerPlayerStats);
    }

    private void Start() {
        isGround = true;
        ChangeState(new IdleState());
        // »½ÐÑ±³°üºÍ²Ëµ¥UIÃæ°å
        // Debug.Log("Íæ¼Ò»½ÐÑUIÃæ°å");
        ResourceManager.Instance.InstantiateResource("Prefabs/Manager/CanvasManager",Vector3.zero,Quaternion.identity);
    }

    void Update() {
        if (Time.timeScale == 0) return;

        currentState?.Update();

        if (controllerPlayerStats.CurrentHealth <= 0 && currentState is not DeathState) {
            ChangeState(new DeathState()); // ÇÐ»»µ½ËÀÍö×´Ì¬
            return;
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
        moveDirection = Vector3.Lerp(moveDirection, Vector3.zero, Time.deltaTime * 5f); // ¼õ»º»÷ÍËÐ§¹û
    }

    public void KnockbackPlayer(Vector3 knockbackForce) {
        knockbackTimer = knockbackDuration;
        moveDirection = knockbackForce;
    }
}
