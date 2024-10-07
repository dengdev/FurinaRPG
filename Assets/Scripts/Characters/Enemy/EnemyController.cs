using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// ȷ������������ھ��� NavMeshAgent �� CharacterStats ����Ķ�����
[RequireComponent(typeof(NavMeshAgent), typeof(CharacterStats))]

public class EnemyController : MonoBehaviour, IGameOverObserver {
    private EnemyState enemyState;
    private NavMeshAgent agent; 
    private Animator animator; 
    private Collider coll; 
    protected CharacterStats enemyData;


    [Header("Basic Settings")]
    public float sightRadius; 
    public bool isGuard; 
    public float lookAtTime; 
    private float enemyMoveSpeed;
    private float remainLookAtTime; // ʣ��Ĺ۲�ʱ��
    protected GameObject attackTarget;

    [Header("Partol State")]
    public float patrolRange;
    private Vector3 patrolPoint;
    private float lastAttackTime; 
    private Vector3 guardPosition; 
    private Quaternion guardRotation; 

    [Header("Alert Settings")]
    public float alertTime = 3f; 
    [SerializeField] private float alertTimer; 

    
    bool isWalking, isChasing, isFollow, isDead;
    bool playerIsDead; 
    private LayerMask playerLayer;

    private Collider[] detectedColliders = new Collider[4]; 

    void Awake() {
        playerLayer = LayerMask.GetMask("Player");

        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        coll = GetComponent<Collider>();
        enemyData = GetComponent<CharacterStats>();

        enemyMoveSpeed = agent.speed; // ��¼��ʼ�ƶ��ٶ�
        guardPosition = transform.position; // ��¼������ʼλ��
        guardRotation = transform.rotation; // ��¼������ʼ����
        remainLookAtTime = lookAtTime; // ��ʼ���۲�ʱ��
    }

    private void Start() {
        enemyState = isGuard ? EnemyState.GUARD : EnemyState.PATROL; // ��ʼ��״̬
        if (!isGuard) {
            GetNewPatrolPoint(); // ��ȡѲ�ߵ�
        }

        // FIXME: �����л��������Ҫ����ע��۲��ߣ�������Ҫ�Ż�
        GameManager.Instance.AddObserver(this);
    }

    // �л�����ʱ����
    private void OnEnable() {
        //GameManager.Instance.AddObserver(this);
    }

    // �������ʱ�Ƴ��۲���
    private void OnDisable() {
        if (!GameManager.IsInitialized) return;
        GameManager.Instance.RemoveObserver(this);
    }

    private void Update() {
        isDead = enemyData.CurrentHealth == 0;

        if (!playerIsDead) {
            UpdateEnemyState(); // �л����˵�״̬
            lastAttackTime -= Time.deltaTime; // ���¹�����ȴ
        }

        UpdateEnemyAnimation(); // ���¶���״̬
    }

    private void UpdateEnemyAnimation() {
        animator.SetBool("Walk", isWalking);
        animator.SetBool("Chase", isChasing);
        animator.SetBool("Follow", isFollow);
        animator.SetBool("Death", isDead);
    }

    private void UpdateEnemyState() {
        if (isDead) {
            enemyState = EnemyState.DEAD;
        } else if (PlayerDetected()) {
            if (enemyState != EnemyState.ALERT && enemyState != EnemyState.CHASE) {
                Debug.Log(this.name + "�������");
                enemyState = EnemyState.ALERT;
                alertTimer = alertTime; // ���þ�����ʱ��
            }
        } else if (enemyState == EnemyState.ALERT) {
            enemyState = isGuard ? EnemyState.GUARD : EnemyState.PATROL;
        }

        switch (enemyState) {
            case EnemyState.GUARD:
                ExecuteGuardBehavior();
                break;
            case EnemyState.PATROL:
                ExecutePatrolBehavior();
                break;
            case EnemyState.ALERT:
                ExecuteAlertBehavior();
                break;
            case EnemyState.CHASE:
                ExecuteChaseBehavior();
                break;
            case EnemyState.DEAD:
                ExecuteEnemyDeadBehavior();
                break;
            default:
                break;
        }
    }
    private void ExecuteAlertBehavior() {
        isChasing = false;

        // ����ʱ�䵹��ʱ
        alertTimer -= Time.deltaTime;
        if (alertTimer <= 0) {
            enemyState = EnemyState.CHASE; // ����ʱ��������л���׷��״̬
        } else {
            // ����״̬�¿���ѡ�����ԭ�صȴ������������
            //agent.destination = transform.position; // ���ֵ�ǰվλ
        }
    }

    private void ExecuteGuardBehavior() {
        isChasing = false;

        if (transform.position != guardPosition) {
            isWalking = true;
            agent.isStopped = false;
            agent.destination = guardPosition;

            // ����ص�����λ�ã�ֹͣ�ƶ����ָ���ʼ����
            if (Vector3.SqrMagnitude(guardPosition - transform.position) <= agent.stoppingDistance) {
                isWalking = false;
                transform.rotation = Quaternion.Lerp(transform.rotation, guardRotation, 0.01f);
            }
        }
    }

    private void ExecutePatrolBehavior() {
        isChasing = false;
        agent.speed = enemyMoveSpeed * 0.5f;

        if (Vector3.Distance(patrolPoint, transform.position) <= agent.stoppingDistance) {
            isWalking = false;
            if (remainLookAtTime > 0)
                remainLookAtTime -= Time.deltaTime;
            else {
                GetNewPatrolPoint();
                remainLookAtTime = lookAtTime;
            }
        } else {
            isWalking = true;
            agent.destination = patrolPoint;
        }
    }

    private void ExecuteChaseBehavior() {
        isWalking = false;
        isChasing = true;

        if (!PlayerDetected()) {
            isFollow = false;
            if (remainLookAtTime > 0) {
                agent.destination = transform.position;
                remainLookAtTime -= Time.deltaTime;
            } else if (isGuard)
                enemyState = EnemyState.GUARD;
            else
                enemyState = EnemyState.PATROL;
        } else {   // ������һ����Ӧʱ��
            isFollow = true;
            agent.isStopped = false;
            agent.destination = attackTarget.transform.position;

            if (WithinAttackRange() || WithinSkillRange()) {
                isFollow = false;
                agent.isStopped = true;
                if (lastAttackTime < 0) {
                    lastAttackTime = enemyData.attackData.coolDown;
                    PerformEnemyAttack(); // ִ�й����߼�
                }
            }
        }
    }

    private void ExecuteEnemyDeadBehavior() {
        // �����󲻻ᵲ·
        agent.radius = 0;
        coll.enabled = false;
        Destroy(gameObject, 2f);
    }

    private void PerformEnemyAttack() {
        enemyData.isCritical = Random.value < enemyData.attackData.criticalChance;
        transform.LookAt(attackTarget.transform);

        if (WithinAttackRange()) {
            animator.SetTrigger("Attack");
        }

        if (WithinSkillRange()) {
            animator.SetTrigger("Skill");
        }
    }

    private bool WithinAttackRange() {
        return attackTarget != null &&
            Vector3.Distance(attackTarget.transform.position, transform.position) <= enemyData.attackData.attackRange;
    }

    private bool WithinSkillRange() {
        return attackTarget != null &&
            Vector3.Distance(attackTarget.transform.position, transform.position) <= enemyData.attackData.skillRange &&
            Vector3.Distance(attackTarget.transform.position, transform.position) > enemyData.attackData.attackRange;
    }

    private bool PlayerDetected() {
        int numColliders = Physics.OverlapSphereNonAlloc(transform.position, sightRadius, detectedColliders, playerLayer);

        for (int i = 0; i < numColliders; i++) {
            if (detectedColliders[i].CompareTag("Player")) {
                attackTarget = detectedColliders[i].gameObject;
                return true;
            }
        }

        attackTarget = null;
        return false;
    }

    private void GetNewPatrolPoint() {
        Vector3 randomPoint = new Vector3(Random.Range(-patrolRange, patrolRange), 0, Random.Range(-patrolRange, patrolRange));
        NavMeshHit hit;
        patrolPoint = guardPosition + randomPoint;

        if (NavMesh.SamplePosition(patrolPoint, out hit, patrolRange, 1)) {
            patrolPoint = hit.position;
        } else {
            patrolPoint = guardPosition;
        }
    }

    /// <summary>
    /// �����е��ø��¼�,������Ч������Ч
    /// </summary>
    private void Hit() {
        // ������ʱ��Ҫ�ж�����Ƿ��ڹ�����Χ��
        if (attackTarget != null && transform.IsFacingTarget(attackTarget.transform)) {
            CharacterStats targetStats = attackTarget.GetComponent<CharacterStats>();
            targetStats.TakeCharacterDamage(enemyData, targetStats); // ִ���˺�
        }
    }

    /// <summary>
    /// �������֮����߼� 
    /// </summary>
    public void PlayerDeadNotify() {
        animator.SetBool("Win", true);
        playerIsDead = true;
        isChasing = false;
        isFollow = false;
        attackTarget = null;
    }


    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, patrolRange);

    }
}
