using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float playerJumpHeight = 2f;
    public float gravity = -9.8f; // 重力加速度
    private bool isRunning = false;

    [Header("Attack Settings")]
    public Weapon weapon;
    public float playerAttackRange;
    public float attackCooldown; // 攻击冷却时间
    public float knockbackDuration = 0.5f; // 击退时间
    private float nextAttackTime; // 下次可攻击的时间
    private GameObject attackTarget; // 攻击目标
    private bool playerIsAttacking = false; // 是否正在攻击
    private bool isMoving = false; // 是否正在移动

    [Header("Interaction Settings")]
    public float interactionDistance; // 交互的最大距离
    private GameObject interactableObject; // 当前可交互的物品

    private Vector3 moveDirection = Vector3.zero; // 角色移动方向
    private float knockbackTimer; // 击退的时间
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
            GameManager.Instance.NotifyObservers(); // 角色死亡进行广播
        }

        if (knockbackTimer > 0) {
            knockbackTimer -= Time.deltaTime;
            characterController.Move(moveDirection * Time.deltaTime); // 根据击退方向进行移动
            moveDirection = Vector3.Lerp(moveDirection, Vector3.zero, Time.deltaTime * 5f); // 减缓击退效果，逐渐停下来

        } else {
            HandlePlayerInput();
        }

        SwitchPlayerAnimation();
        playerIsAttacking = animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"); // 检查当前是否在播放攻击动画
        isMoving = moveDirection != Vector3.zero;

        // 攻击冷却时间减少
        nextAttackTime -= Time.deltaTime;

        PlayerCheckForInteractable();
        PlayerFoundEnemy();
    }
    private void HandlePlayerInput() {
        if (Input.GetMouseButtonDown(0) && nextAttackTime <= 0) {
            if (!isMoving) {
                StartCoroutine(PlayerAttack());
            } else {
                // 如果正在移动，取消移动并开始攻击
                moveDirection = Vector3.zero;
                StartCoroutine(PlayerAttack());
            }
        } else if (!playerIsAttacking) {
            // 只有在没有攻击时才能移动
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
        nextAttackTime = playerStats.attackData.coolDown; // 重置攻击冷却时间

        // 等待攻击动画中的关键帧自动启用武器碰撞检测
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

            transform.forward = moveDirection; // 角色面朝移动方向
        } else {
            moveDirection = Vector3.zero;
            currentMoveSpeed = 0f;
        }

        if (Input.GetButtonDown("Jump")) {
            jumpVelocity = Mathf.Sqrt(2 * playerJumpHeight * -gravity); // 根据跳跃高度计算跳跃速度
            animator.SetTrigger("Jump");
        }

        // 计算水平移动
        Vector3 horizontalMovement = moveDirection * currentMoveSpeed * Time.deltaTime;
        // 应用重力到跳跃速度
        jumpVelocity += gravity * Time.deltaTime;
        characterController.Move(horizontalMovement + Vector3.up * jumpVelocity * Time.deltaTime);
    }

    private void SwitchPlayerAnimation() {
        animator.SetFloat("Speed", currentMoveSpeed); // 更新速度动画参数
        animator.SetBool("Death", playerStats.CurrentHealth == 0); // 更新死亡动画参数
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
                    rock.rockState = Rock.RockStates.HitEnemy; // 设置岩石为攻击敌人
                    Rigidbody rb = attackTarget.GetComponent<Rigidbody>();
                    rb.velocity = Vector3.one; // 击飞的初始速度
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
            // 不使用此方式找敌人
            if (target.CompareTag("Attackable")) {
                attackTarget = target.gameObject;
                Debug.Log($"找到攻击目标: {attackTarget.name}");
                return true;
            }
        }
        attackTarget = null;
        return false;
    }

    /// <summary>
    /// 显示攻击范围（用于调试）
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
