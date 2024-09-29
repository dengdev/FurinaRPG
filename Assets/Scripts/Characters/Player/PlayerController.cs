using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    [Header("��ɫ״̬")]
    private float walkSpeed = 4f;
    private float runSpeed = 8f;
    public float playerJumpHeight = 2f;
    public float currentMoveSpeed;
    public bool isGround;
    public bool playerIsAttacking;
    public bool playerIsHIt;
    public bool playerIsDizzy;

    public float jumpVelocity;
    public bool isRunning = false;
    public Vector3 moveDirection = Vector3.zero;

    [SerializeField] public List<Item> items;

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
    public CharacterStats playerStats;
    private IPlayerState currentState;

    private void Awake() {
        animator = GetComponent<Animator>();
        playerStats = GetComponent<CharacterStats>();
        cameraTransform = Camera.main.transform;
        characterController = GetComponent<CharacterController>();

    }

    private void OnEnable() {
        Debug.Log("��ҿ�������ͼע��");
        GameManager.Instance.RegisterPlayer(playerStats);
    }

    void Start() {
        Debug.Log("��ҿ����������������,������ͼ�ٴ�ע��");
        SaveManager.Instance.Load();
        GameManager.Instance.RegisterPlayer(playerStats);
        
        isGround = true;
        ChangeState(new IdleState());

    }

    void Update() {
        currentState?.Update();

        if (playerStats.CurrentHealth <= 0 && currentState is not DeathState) {
            ChangeState(new DeathState()); // �л�������״̬
            return;
        }

        if (Input.GetKeyDown(KeyCode.F)) {
            Debug.Log($"����F����ͼ���IDΪ4����Ʒ��������{SaveManager.Instance.allItems[4].item_Name}");

            if (playerStats.characterData.items == null) {
                playerStats.characterData.items=GameManager.Instance.playerStats.characterData.items;
                Debug.Log("��ҵı���Ϊ��");
                playerStats.characterData.items = new List<Item>();
            }
            playerStats.characterData.items.Add(SaveManager.Instance.allItems[4]);
            Debug.Log("����Ϸ�����0�ź�4����Ʒ");
            GameManager.Instance.playerStats.characterData.items.Add(SaveManager.Instance.allItems[1]);

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
        moveDirection = Vector3.Lerp(moveDirection, Vector3.zero, Time.deltaTime * 5f); // ��������Ч��
    }

    public void KnockbackPlayer(Vector3 knockbackForce) {
        knockbackTimer = knockbackDuration;
        moveDirection = knockbackForce;
    }
}
