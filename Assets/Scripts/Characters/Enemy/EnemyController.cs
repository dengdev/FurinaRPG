using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// 确保该组件附加在具有 NavMeshAgent 和 CharacterStats 组件的对象上
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
    private float remainLookAtTime; // 剩余的观察时间
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

        enemyMoveSpeed = agent.speed; // 记录初始移动速度
        guardPosition = transform.position; // 记录守卫初始位置
        guardRotation = transform.rotation; // 记录守卫初始朝向
        remainLookAtTime = lookAtTime; // 初始化观察时间
    }

    private void Start() {
        enemyState = isGuard ? EnemyState.GUARD : EnemyState.PATROL; // 初始化状态
        if (!isGuard) {
            GetNewPatrolPoint(); // 获取巡逻点
        }

        // FIXME: 场景切换后可能需要重新注册观察者，这里需要优化
        GameManager.Instance.AddObserver(this);
    }

    // 切换场景时启用
    private void OnEnable() {
        //GameManager.Instance.AddObserver(this);
    }

    // 对象禁用时移除观察者
    private void OnDisable() {
        if (!GameManager.IsInitialized) return;
        GameManager.Instance.RemoveObserver(this);
    }

    private void Update() {
        isDead = enemyData.CurrentHealth == 0;

        if (!playerIsDead) {
            UpdateEnemyState(); // 切换敌人的状态
            lastAttackTime -= Time.deltaTime; // 更新攻击冷却
        }

        UpdateEnemyAnimation(); // 更新动画状态
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
                Debug.Log(this.name + "发现玩家");
                enemyState = EnemyState.ALERT;
                alertTimer = alertTime; // 重置警觉计时器
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

        // 警觉时间倒计时
        alertTimer -= Time.deltaTime;
        if (alertTimer <= 0) {
            enemyState = EnemyState.CHASE; // 警觉时间结束，切换到追击状态
        } else {
            // 警觉状态下可以选择继续原地等待或缓慢靠近玩家
            //agent.destination = transform.position; // 保持当前站位
        }
    }

    private void ExecuteGuardBehavior() {
        isChasing = false;

        if (transform.position != guardPosition) {
            isWalking = true;
            agent.isStopped = false;
            agent.destination = guardPosition;

            // 如果回到守卫位置，停止移动并恢复初始朝向
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
        } else {   // 给怪物一个反应时间
            isFollow = true;
            agent.isStopped = false;
            agent.destination = attackTarget.transform.position;

            if (WithinAttackRange() || WithinSkillRange()) {
                isFollow = false;
                agent.isStopped = true;
                if (lastAttackTime < 0) {
                    lastAttackTime = enemyData.attackData.coolDown;
                    PerformEnemyAttack(); // 执行攻击逻辑
                }
            }
        }
    }

    private void ExecuteEnemyDeadBehavior() {
        // 死亡后不会挡路
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
    /// 动画中调用该事件,触发特效或者音效
    /// </summary>
    private void Hit() {
        // 攻击的时候要判断玩家是否在攻击范围内
        if (attackTarget != null && transform.IsFacingTarget(attackTarget.transform)) {
            CharacterStats targetStats = attackTarget.GetComponent<CharacterStats>();
            targetStats.TakeCharacterDamage(enemyData, targetStats); // 执行伤害
        }
    }

    /// <summary>
    /// 玩家死亡之后的逻辑 
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
