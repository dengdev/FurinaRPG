using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// �����ɫ����
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpHeight = 2f; //��Ծ�߶�
    public float gravity = -9.8f; // �������ٶ�
    private bool isRunning = false;

    [Header("Attack Settings")]
    public Weapon weapon;
    public float attackRange; // ������Χ
    public float attackCooldown; // ������ȴʱ��
    public float knockbackDuration = 0.5f; // ����ʱ��
    private float nextAttackTime; // �´οɹ�����ʱ��
    private GameObject attackTarget; // ����Ŀ��
    private bool isAttacking = false; // �Ƿ����ڹ���
    private bool isMoving = false; // �Ƿ������ƶ�

    [Header("Interaction Settings")]
    public float interactionDistance; // ������������
    private GameObject interactableObject; // ��ǰ�ɽ�������Ʒ

    private Vector3 moveDirection = Vector3.zero; // ��ɫ�ƶ�����
    private float knockbackTimer; // ���˵�ʱ��
    private float jumpVelocity;

    public float currentMoveSpeed;

    private Transform cameraTransform; // �����Transform
    private CharacterController characterController; // ��ɫ������
    private Animator animator; // ����������
    private CharacterStats playerStats; // ��ҽ�ɫ������

    public CharacterStats GetplayerStats {  get { return playerStats; } }

    private void Awake()
    {
        animator = GetComponent<Animator>(); // ��ȡ�������������
        playerStats = GetComponent<CharacterStats>(); // ��ȡ��ɫ״̬���
        cameraTransform = Camera.main.transform; // ��ȡ�������Transform
        characterController = GetComponent<CharacterController>(); // ��ȡ��ɫ���������
    }

    void Start()
    {
        // ��ʼ�������Ĺ�����
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
            GameManager.Instance.NotifyObservers(); // ��ɫ�������й㲥
        }

        if (knockbackTimer > 0)
        {
            knockbackTimer -= Time.deltaTime;
            characterController.Move(moveDirection * Time.deltaTime); // ���ݻ��˷�������ƶ�
            moveDirection = Vector3.Lerp(moveDirection, Vector3.zero, Time.deltaTime * 5f); // ��������Ч������ͣ����

        }
        else
        {
            HandleInput();
        }

        SwitchAnimation();
        isAttacking = animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"); // ��鵱ǰ�Ƿ��ڲ��Ź�������
        isMoving = moveDirection != Vector3.zero;

        // ������ȴʱ�����
        nextAttackTime -= Time.deltaTime;

        // ��齻���͹���Ŀ��
        CheckForInteractable();
        HandleInteraction();
        FoundEnemy();
    }

    /// <summary>
    /// �����������
    /// </summary>
    private void HandleInput()
    {
        // ��鹥������
        if (Input.GetMouseButtonDown(0) && nextAttackTime <= 0)
        {
            if (!isMoving)
            {
                StartCoroutine(Attack());
            }
            else
            {
                // ��������ƶ���ȡ���ƶ�����ʼ����
                moveDirection = Vector3.zero;
                StartCoroutine(Attack());
            }
        }
        else if (!isAttacking)
        {
            // ֻ����û�й���ʱ�����ƶ�
            Move();
        }
    }

    /// <summary>
    /// ʵ�ֱ����˵��߼�
    /// </summary>
    /// <param name="knockbackDirection"></param>
    public void Knockback(Vector3 knockbackForce)
    {
        // Ӧ�û��˵ķ��������
        knockbackTimer = knockbackDuration;
        moveDirection = knockbackForce; // ���û��˷���
    }

    /// <summary>
    /// ��ɫ�����߼�
    /// </summary>
    /// <returns></returns>
    private IEnumerator Attack()
    {
        // ����Ҫ��ɫ�Զ��泯��������
        //if(attackTarget!=null) transform.LookAt(attackTarget.transform.position);

        isAttacking = true; // ��ʼ�����߼�
        playerStats.isCritical = UnityEngine.Random.value < playerStats.attackData.criticalChance; // �����ж�
        animator.SetTrigger("Attack"); // ������������
        nextAttackTime = playerStats.attackData.coolDown; // ���ù�����ȴʱ��

        // �ȴ����������еĹؼ�֡�Զ�����������ײ���
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        isAttacking = false;
    }

    /// <summary>
    /// ��ɫ�ƶ��߼�
    /// </summary>
    private void Move()
    {
        if (playerStats.CurrentHealth == 0) return;
        // ��ȡ�������
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // ��ȡ�����ǰ������ҷ���
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

            // �л��ٶ��߼�
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                isRunning = !isRunning;
            }
            currentMoveSpeed = isRunning ? runSpeed : walkSpeed;

            transform.forward = moveDirection; // ��ɫ�泯�ƶ�����
        }
        else
        {
            moveDirection = Vector3.zero;
            currentMoveSpeed = 0f;
        }

        // ��ɫ��Ծ�߼�
        if (Input.GetButtonDown("Jump"))
        {
            jumpVelocity = Mathf.Sqrt(2 * jumpHeight * -gravity); // ������Ծ�߶ȼ�����Ծ�ٶ�
            animator.SetTrigger("Jump");
        }

        // ����ˮƽ�ƶ�
        Vector3 horizontalMovement = moveDirection * currentMoveSpeed * Time.deltaTime;
        // Ӧ����������Ծ�ٶ�
        jumpVelocity += gravity * Time.deltaTime;
        // �ƶ���ɫ
        characterController.Move(horizontalMovement + Vector3.up * jumpVelocity * Time.deltaTime);  
    }

    /// <summary>
    /// �л���Ҷ���
    /// </summary>
    private void SwitchAnimation()
    {
        animator.SetFloat("Speed", currentMoveSpeed); // �����ٶȶ�������
        animator.SetBool("Death", playerStats.CurrentHealth == 0); // ����������������
    }

    /// <summary>
    /// ������ǰ���Ƿ��пɽ�������Ʒ
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
    /// ������ҽ����߼�
    /// </summary>
    private void HandleInteraction()
    {
        if (interactableObject != null && Input.GetKeyDown(KeyCode.F))
        {
            interactableObject.GetComponent<Interactive>().Interact();
        }
    }

    /// <summary>
    /// �����¼��������е���
    /// </summary>
    private void Hit()
    { // ������ʯ
        if (attackTarget != null)
        {
            if (attackTarget.CompareTag("Attackable"))
            {
                Rock rock = attackTarget.GetComponent<Rock>();
                if (rock != null)
                {
                    rock.rockState = Rock.RockStates.HitEnemy; // ������ʯΪ��������
                    Rigidbody rb = attackTarget.GetComponent<Rigidbody>();
                    rb.velocity = Vector3.one; // ���ɵĳ�ʼ�ٶ�
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
    /// �ڹ���������Ѱ�ҵ���λ��
    /// </summary>
    private bool FoundEnemy()
    {
        var colliders = Physics.OverlapSphere(transform.position, attackRange+2);
        foreach (var target in colliders)
        {
            // ��ʹ�ô˷�ʽ�ҵ���
            if (target.CompareTag("Attackable"))
            {
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
