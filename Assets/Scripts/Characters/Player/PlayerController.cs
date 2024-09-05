using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 处理角色控制
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpHeight = 2f; //跳跃高度
    public float gravity = -9.8f; // 重力加速度
    private bool isRunning = false;

    [Header("Attack Settings")]
    public Weapon weapon;
    public float attackRange; // 攻击范围
    public float attackCooldown; // 攻击冷却时间
    public float knockbackDuration = 0.5f; // 击退时间
    private float nextAttackTime; // 下次可攻击的时间
    private GameObject attackTarget; // 攻击目标
    private bool isAttacking = false; // 是否正在攻击
    private bool isMoving = false; // 是否正在移动

    [Header("Interaction Settings")]
    public float interactionDistance; // 交互的最大距离
    private GameObject interactableObject; // 当前可交互的物品

    private Vector3 moveDirection = Vector3.zero; // 角色移动方向
    private float knockbackTimer;
    private float jumpVelocity;

    private Vector3 knockbackDirection;
    public float currentMoveSpeed;

    private Transform cameraTransform; // 相机的Transform
    private CharacterController characterController; // 角色控制器
    private Animator animator; // 动画控制器
    private CharacterStats playerStats; // 玩家角色的属性

    public CharacterStats GetplayerStats {  get { return playerStats; } }

    private void Awake()
    {
        animator = GetComponent<Animator>(); // 获取动画控制器组件
        playerStats = GetComponent<CharacterStats>(); // 获取角色状态组件
        cameraTransform = Camera.main.transform; // 获取主相机的Transform
        characterController = GetComponent<CharacterController>(); // 获取角色控制器组件
    }

    void Start()
    {
        // 初始化武器的攻击者
        if (weapon != null)
        {
            weapon.SetAttacker(this);
        }
        GameManager.Instance.RegisterPlayer(playerStats);
    }

    void Update()
    {

        if (playerStats.CurrentHealth == 0)
        {
            GameManager.Instance.NotifyObservers(); // 角色死亡进行广播
        }

        if (knockbackTimer > 0)
        {
            knockbackTimer -= Time.deltaTime;
            moveDirection = Vector3.zero;
        }
        else
        {
            HandleInput();
        }

        SwitchAnimation();
        isAttacking = animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"); // 检查当前是否在播放攻击动画
        isMoving = moveDirection != Vector3.zero;

        // 攻击冷却时间减少
        nextAttackTime -= Time.deltaTime;

        // 检查交互和攻击目标
        CheckForInteractable();
        HandleInteraction();
        FoundEnemy();
    }

    /// <summary>
    /// 处理玩家输入
    /// </summary>
    private void HandleInput()
    {
        // 检查攻击输入
        if (Input.GetMouseButtonDown(0) && nextAttackTime <= 0)
        {
            if (!isMoving)
            {
                StartCoroutine(Attack());
            }
            else
            {
                // 如果正在移动，取消移动并开始攻击
                moveDirection = Vector3.zero;
                StartCoroutine(Attack());
            }
        }
        else if (!isAttacking)
        {
            // 只有在没有攻击时才能移动
            Move();
        }
    }

    /// <summary>
    /// 实现被击退的逻辑
    /// </summary>
    /// <param name="knockbackDirection"></param>
    public void Knockback(Vector3 knockbackDirection)
    {
        this.knockbackDirection = knockbackDirection;
        knockbackTimer = knockbackDuration;
    }

    /// <summary>
    /// 角色攻击逻辑
    /// </summary>
    /// <returns></returns>
    private IEnumerator Attack()
    {
        //角色面朝攻击对象
        if(attackTarget!=null) transform.LookAt(attackTarget.transform.position);

        isAttacking = true; // 开始攻击逻辑
        playerStats.isCritical = UnityEngine.Random.value < playerStats.attackData.criticalChance; // 暴击判断
        animator.SetTrigger("Attack"); // 触发攻击动画
        nextAttackTime = playerStats.attackData.coolDown; // 重置攻击冷却时间

        // 等待攻击动画中的关键帧自动启用武器碰撞检测
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        isAttacking = false;
    }

    /// <summary>
    /// 角色移动逻辑
    /// </summary>
    private void Move()
    {
        if (playerStats.CurrentHealth == 0) return;
        // 获取玩家输入
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // 获取相机的前方向和右方向
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0f; 
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 desiredMoveDirection = forward * vertical + right * horizontal;

        if (desiredMoveDirection != Vector3.zero)
        {
            moveDirection=desiredMoveDirection;

            // 切换速度逻辑
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                isRunning = !isRunning;
            }
            currentMoveSpeed = isRunning ? runSpeed : walkSpeed;

            transform.forward = moveDirection; // 角色面朝移动方向
        }
        else
        {
            moveDirection = Vector3.zero;
            currentMoveSpeed = 0f;
        }

        // 角色跳跃逻辑
        if (Input.GetButtonDown("Jump"))
        {
            jumpVelocity = Mathf.Sqrt(2 * jumpHeight * -gravity); // 根据跳跃高度计算跳跃速度
            animator.SetTrigger("Jump");
        }

        // 计算水平移动
        Vector3 horizontalMovement = moveDirection * currentMoveSpeed * Time.deltaTime;
        // 应用重力到跳跃速度
        jumpVelocity += gravity * Time.deltaTime;
        // 移动角色
        characterController.Move(horizontalMovement + Vector3.up * jumpVelocity * Time.deltaTime);  
    }

    /// <summary>
    /// 切换玩家动画
    /// </summary>
    private void SwitchAnimation()
    {
        animator.SetFloat("Speed", currentMoveSpeed); // 更新速度动画参数
        animator.SetBool("Death", playerStats.CurrentHealth == 0); // 更新死亡动画参数
    }

    /// <summary>
    /// 检测玩家前方是否有可交互的物品
    /// </summary>
    private void CheckForInteractable()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, interactionDistance))
        {
            interactableObject = hit.collider.CompareTag("Interactable") ? hit.collider.gameObject : null;
        }
        else
        {
            interactableObject = null;
        }
    }

    /// <summary>
    /// 处理玩家交互逻辑
    /// </summary>
    private void HandleInteraction()
    {
        if (interactableObject != null && Input.GetKeyDown(KeyCode.F))
        {
            interactableObject.GetComponent<Interactive>().Interact();
        }
    }

    /// <summary>
    /// 动画事件，动画中调用
    /// </summary>
    private void Hit()
    {
        if (attackTarget != null)
        {
            // 攻击石头
            if (attackTarget.CompareTag("Attackable"))
            {
                Rock rock = attackTarget.GetComponent<Rock>();
                if (rock != null)
                {
                    rock.rockState = Rock.RockStates.HitEnemy;
                    Rigidbody rb = attackTarget.GetComponent<Rigidbody>();
                    rb.velocity = Vector3.one;
                    rb.AddForce(transform.forward * 20, ForceMode.Impulse);
                }
            }
            else
            {
                var targetStats = attackTarget.GetComponent<CharacterStats>();
                targetStats.TakeDamage(playerStats, targetStats);
            }
        }
    }

    /// <summary>
    /// 在攻击距离内寻找敌人位置
    /// </summary>
    private bool FoundEnemy()
    {
        var colliders = Physics.OverlapSphere(transform.position, attackRange+2);
        foreach (var target in colliders)
        {
            // 不使用此方式找敌人
            if (target.CompareTag("Attackable"))
            {
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
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    public void EnableWeaponCollider()
    {
        weapon.EnableWeaponCollider();
    }
    public void DisableWeaponCollider()
    {
        weapon.DisableWeaponCollider();
    }
}
