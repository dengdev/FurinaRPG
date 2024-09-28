using System;
using System.Collections;
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

    //private Canvas playerHPCanvas;

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
    public float knockbackDuration = 0.5f; 
    private float knockbackTimer; 

    public  Transform cameraTransform;
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
        Debug.Log("�����������");
        SaveManager.Instance.LoadPlayerData();
        Debug.Log("��ҿ�������ͼ�ٴ�ע��");
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

        if (knockbackTimer > 0) {
            HandleKnockback();
        }
    }

    public bool NowIsHitState() {
        return currentState is HitState ;
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

    //private void ShowHPBar() {
    //    // ͨ��������Ⱦģʽ�ҵ�Ѫ�����صĻ�����
    //    foreach (Canvas canvas in FindObjectsOfType<Canvas>()) {
    //        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay) {
    //            playerHPCanvas = canvas;
    //            playerHPCanvas.transform.GetChild(0).gameObject.SetActive(true);
    //        }
    //    }

    //}
}
