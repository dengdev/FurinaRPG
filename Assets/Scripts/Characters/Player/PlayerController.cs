using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float playerJumpHeight = 2f;
    public float gravity = -9.8f; // �������ٶ�
    private bool isRunning = false;

    [Header("Attack Settings")]
    public Weapon weapon;
    public float playerAttackRange;
    public float attackCooldown; // ������ȴʱ��
    public float knockbackDuration = 0.5f; // ����ʱ��
    private float nextAttackTime; // �´οɹ�����ʱ��
    private GameObject attackTarget; // ����Ŀ��
    private bool playerIsAttacking = false; // �Ƿ����ڹ���
    private bool isMoving = false; // �Ƿ������ƶ�

    [Header("Interaction Settings")]
    public float interactionDistance; // ������������
    private GameObject interactableObject; // ��ǰ�ɽ�������Ʒ

    private Vector3 moveDirection = Vector3.zero; // ��ɫ�ƶ�����
    private float knockbackTimer; // ���˵�ʱ��
    private float jumpVelocity;

    public float currentMoveSpeed;

    private Transform cameraTransform;
    private CharacterController characterController;
    private Animator animator;
    private CharacterStats playerStats;

    public CharacterStats GetplayerStats { get { return playerStats; } }

    private void Awake() {
        animator = GetComponent<Animator>();
        playerStats = GetComponent<CharacterStats>();
        cameraTransform = Camera.main.transform;
        GameManager.Instance.RegisterPlayer(playerStats);
        characterController = GetComponent<CharacterController>();
    }
    private void OnEnable() {
        GameManager.Instance.RegisterPlayer(playerStats);
    }

    void Start() {
        if (weapon != null) {
            weapon.SetAttacker(this);
        }
        SaveManager.Instance.LoadPlayerData();
    }

    void Update() {
        if (playerStats.CurrentHealth == 0) {
            GameManager.Instance.NotifyObservers(); // ��ɫ�������й㲥
        }

        if (knockbackTimer > 0) {
            knockbackTimer -= Time.deltaTime;
            characterController.Move(moveDirection * Time.deltaTime); // ���ݻ��˷�������ƶ�
            moveDirection = Vector3.Lerp(moveDirection, Vector3.zero, Time.deltaTime * 5f); // ��������Ч������ͣ����

        } else {
            HandlePlayerInput();
        }

        SwitchPlayerAnimation();
        playerIsAttacking = animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"); // ��鵱ǰ�Ƿ��ڲ��Ź�������
        isMoving = moveDirection != Vector3.zero;

        // ������ȴʱ�����
        nextAttackTime -= Time.deltaTime;

        PlayerCheckForInteractable();
        PlayerFoundEnemy();
    }
    private void HandlePlayerInput() {
        if (Input.GetMouseButtonDown(0) && nextAttackTime <= 0) {
            if (!isMoving) {
                StartCoroutine(PlayerAttack());
            } else {
                // ��������ƶ���ȡ���ƶ�����ʼ����
                moveDirection = Vector3.zero;
                StartCoroutine(PlayerAttack());
            }
        } else if (!playerIsAttacking) {
            // ֻ����û�й���ʱ�����ƶ�
            PlayerMove();
        }
    }

    public void KnockbackPlayer(Vector3 knockbackForce) {
        knockbackTimer = knockbackDuration;
        moveDirection = knockbackForce;
    }

    private IEnumerator PlayerAttack() {
        playerIsAttacking = true;
        playerStats.isCritical = UnityEngine.Random.value < playerStats.attackData.criticalChance;
        animator.SetTrigger("Attack");
        nextAttackTime = playerStats.attackData.coolDown; // ���ù�����ȴʱ��

        // �ȴ����������еĹؼ�֡�Զ�����������ײ���
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        playerIsAttacking = false;
    }
    private void PlayerMove() {
        if (playerStats.CurrentHealth == 0) return;
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 desiredMoveDirection = forward * vertical + right * horizontal;

        if (desiredMoveDirection != Vector3.zero) {
            moveDirection = desiredMoveDirection;

            if (Input.GetKeyDown(KeyCode.LeftShift)) {
                isRunning = !isRunning;
            }
            currentMoveSpeed = isRunning ? runSpeed : walkSpeed;

            transform.forward = moveDirection; // ��ɫ�泯�ƶ�����
        } else {
            moveDirection = Vector3.zero;
            currentMoveSpeed = 0f;
        }

        if (Input.GetButtonDown("Jump")) {
            jumpVelocity = Mathf.Sqrt(2 * playerJumpHeight * -gravity); // ������Ծ�߶ȼ�����Ծ�ٶ�
            animator.SetTrigger("Jump");
        }

        // ����ˮƽ�ƶ�
        Vector3 horizontalMovement = moveDirection * currentMoveSpeed * Time.deltaTime;
        // Ӧ����������Ծ�ٶ�
        jumpVelocity += gravity * Time.deltaTime;
        characterController.Move(horizontalMovement + Vector3.up * jumpVelocity * Time.deltaTime);
    }

    private void SwitchPlayerAnimation() {
        animator.SetFloat("Speed", currentMoveSpeed); // �����ٶȶ�������
        animator.SetBool("Death", playerStats.CurrentHealth == 0); // ����������������
    }

    private void PlayerCheckForInteractable() {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, interactionDistance)) {
            interactableObject = hit.collider.CompareTag("Interactable") ? hit.collider.gameObject : null;
        } else {
            interactableObject = null;
        }
    }

    private void HitRockOnAnimation() {
        if (attackTarget != null) {
            if (attackTarget.CompareTag("Attackable")) {
                Rock rock = attackTarget.GetComponent<Rock>();
                if (rock != null) {
                    rock.rockState = Rock.RockStates.HitEnemy; // ������ʯΪ��������
                    Rigidbody rb = attackTarget.GetComponent<Rigidbody>();
                    rb.velocity = Vector3.one; // ���ɵĳ�ʼ�ٶ�
                    rb.AddForce(transform.forward * 20, ForceMode.Impulse);
                }
            } else {
                var targetStats = attackTarget.GetComponent<CharacterStats>();
                targetStats.TakeCharacterDamage(playerStats, targetStats);
            }
        }
    }

    private bool PlayerFoundEnemy() {
        var colliders = Physics.OverlapSphere(transform.position, playerAttackRange + 2);
        foreach (var target in colliders) {
            // ��ʹ�ô˷�ʽ�ҵ���
            if (target.CompareTag("Attackable")) {
                attackTarget = target.gameObject;
                Debug.Log($"�ҵ�����Ŀ��: {attackTarget.name}");
                return true;
            }
        }
        attackTarget = null;
        return false;
    }

    /// <summary>
    /// ��ʾ������Χ�����ڵ��ԣ�
    /// </summary>
    private void OnDrawGizmos() {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, playerAttackRange);
    }

    public void EnableWeaponCollider() {
        weapon.EnableWeaponCollider();
    }
    public void DisableWeaponCollider() {
        weapon.DisableWeaponCollider();
    }
}
