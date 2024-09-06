using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// 确保该组件附加在具有 NavMeshAgent 和 CharacterStats 组件的对象上
[RequireComponent(typeof(NavMeshAgent), typeof(CharacterStats))]

public class EnemyController : MonoBehaviour, IEndGameObserver
{
    private EnemyState enemyState; // 敌人的当前状态 (巡逻、追击、守卫等)
    private NavMeshAgent agent; // 用于路径导航的组件
    private Animator animator; // 控制敌人动画的组件
    private Collider coll; // 敌人的碰撞体，用于检测碰撞
    protected CharacterStats enemyStats; // 敌人的属性数据 (生命值、攻击力等)

    [Header("Basic Settings")]
    public float sightRadius; // 敌人的视野范围
    public bool isGuard; // 判断敌人是守卫模式还是巡逻模式
    public float lookAtTime; // 敌人停止移动后观察环境的时间
    private float movementSpeed; // 敌人的移动速度
    private float remainLookAtTime; // 剩余的观察时间
    protected GameObject attackTarget; // 当前攻击目标

    [Header("Partol State")]
    public float patrolRange; // 巡逻范围
    private Vector3 patrolPoint; // 巡逻的目标点
    private float lastAttackTime; // 上次攻击的时间，用于控制攻击间隔
    private Vector3 guardPosition; // 守卫模式下敌人的初始位置
    private Quaternion guardRotation; // 守卫模式下敌人的初始朝向

    [Header("Alert Settings")]
    public float alertTime = 1.2f; // 警觉时间
    private float alertTimer; // 当前的警觉计时器

    // 控制动画状态的布尔值
    bool isWalking, isChasing, isFollow, isDead;
    bool playerDead; // 玩家是否死亡

    // 初始化组件和变量
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        coll = GetComponent<Collider>();
        enemyStats = GetComponent<CharacterStats>();

        movementSpeed = agent.speed; // 记录初始移动速度
        guardPosition = transform.position; // 记录守卫初始位置
        guardRotation = transform.rotation; // 记录守卫初始朝向
        remainLookAtTime = lookAtTime; // 初始化观察时间
    }

    // 开始时设置敌人状态
    private void Start()
    {
        enemyState = isGuard ? EnemyState.GUARD : EnemyState.PATROL; // 初始化状态
        if (!isGuard)
        {
            GetNewPatrolPoint(); // 获取巡逻点
        }

        // FIXME: 场景切换后可能需要重新注册观察者，这里需要优化
        GameManager.Instance.AddObserver(this);
    }

    // 切换场景时启用
    private void OnEnable()
    {
        //GameManager.Instance.AddObserver(this);
    }

    // 对象禁用时移除观察者
    private void OnDisable()
    {
        if (!GameManager.IsInitialized) return;
        GameManager.Instance.RemoveObserver(this);
    }

    private void Update()
    {
        isDead = enemyStats.CurrentHealth == 0; // 判断敌人是否死亡

        if (!playerDead)
        {
            UpdateState(); // 切换敌人的状态
            lastAttackTime -= Time.deltaTime; // 更新攻击冷却
        }

        UpdateAnimation(); // 更新动画状态
    }

    private void UpdateAnimation()
    {
        // 设置动画参数，控制敌人动画
        animator.SetBool("Walk", isWalking);
        animator.SetBool("Chase", isChasing);
        animator.SetBool("Follow", isFollow);
        animator.SetBool("Death", isDead);
    }

    /// <summary>
    /// 根据敌人状态执行对应的逻辑
    /// </summary>
    private void UpdateState()
    {
        if (isDead)
        {   // 如果敌人死亡，切换到死亡状态
            enemyState = EnemyState.DEAD;
        }
        else if (PlayerDetected())
        {
            if (enemyState != EnemyState.ALERT && enemyState != EnemyState.CHASE) // 发现玩家后进入警觉状态
            {
                Debug.Log(this.name + "发现玩家");
                enemyState = EnemyState.ALERT;
                alertTimer = alertTime; // 重置警觉计时器
            }
        }
        else if (enemyState == EnemyState.ALERT)
        {
            // 如果玩家脱离视野范围，恢复到默认状态
            enemyState = isGuard ? EnemyState.GUARD : EnemyState.PATROL;
        }

        switch (enemyState)
        {
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
                ExecuteDeadBehavior();
                break;
            default:
                break;
        }
    }
    private void ExecuteAlertBehavior()
    {
        isChasing = false;

        // 警觉时间倒计时
        alertTimer -= Time.deltaTime;
        if (alertTimer <= 0)
        {
            enemyState = EnemyState.CHASE; // 警觉时间结束，切换到追击状态
        }
        else
        {
            // 警觉状态下可以选择继续原地等待或缓慢靠近玩家
            //agent.destination = transform.position; // 保持当前站位
        }
    }

    /// <summary>
    /// 守卫状态的逻辑
    /// </summary>
    private void ExecuteGuardBehavior()
    {
        isChasing = false;

        if (transform.position != guardPosition)
        {
            isWalking = true;
            agent.isStopped = false;
            agent.destination = guardPosition;

            // 如果回到守卫位置，停止移动并恢复初始朝向
            if (Vector3.SqrMagnitude(guardPosition - transform.position) <= agent.stoppingDistance)
            {
                isWalking = false;
                transform.rotation = Quaternion.Lerp(transform.rotation, guardRotation, 0.01f);
            }
        }
    }

    /// <summary>
    /// 巡逻状态的逻辑
    /// </summary>
    private void ExecutePatrolBehavior()
    {
        isChasing = false;
        agent.speed = movementSpeed * 0.5f; // 巡逻时减慢速度

        // 判断是否到达巡逻点
        if (Vector3.Distance(patrolPoint, transform.position) <= agent.stoppingDistance)
        {
            isWalking = false;
            if (remainLookAtTime > 0)
                remainLookAtTime -= Time.deltaTime; // 停留一段时间后再移动到下一个巡逻点
            else
            {
                GetNewPatrolPoint();
                remainLookAtTime = lookAtTime;
            }
        }
        else
        {
            isWalking = true;
            agent.destination = patrolPoint;
        }
    }

    /// <summary>
    /// 追击时，根据距离触发攻击或者技能
    /// </summary>
    private void ExecuteChaseBehavior()
    {
        isWalking = false;
        isChasing = true;

        if (!PlayerDetected())
        {
            isFollow = false;
            if (remainLookAtTime > 0)
            {
                agent.destination = transform.position;
                remainLookAtTime -= Time.deltaTime;
            }
            else if (isGuard)
                enemyState = EnemyState.GUARD;
            else
                enemyState = EnemyState.PATROL;
        }
        else
        {   // 给怪物一个反应时间
            isFollow = true;
            agent.isStopped = false;
            agent.destination = attackTarget.transform.position;

            if (WithinAttackRange() || WithinSkillRange())
            {
                isFollow = false;
                agent.isStopped = true;
                if (lastAttackTime < 0)
                {   // 可以攻击
                    lastAttackTime = enemyStats.attackData.coolDown;
                    PerformAttack(); // 执行攻击逻辑
                }
            }
        }
    }

    /// <summary>
    /// 执行敌人死亡的逻辑
    /// </summary>
    private void ExecuteDeadBehavior()
    {
        // 死亡后不会挡路
        agent.radius = 0;
        coll.enabled = false;
        Destroy(gameObject, 2f); // 2秒后销毁敌人对象
    }

    /// <summary>
    /// 执行敌人攻击
    /// </summary>
    void PerformAttack()
    {
        // 判断是否触发暴击
        enemyStats.isCritical = Random.value < enemyStats.attackData.criticalChance;

        // 面向攻击目标
        transform.LookAt(attackTarget.transform);

        if (WithinAttackRange())
        {
            animator.SetTrigger("Attack"); // 播放攻击动画
        }

        if (WithinSkillRange())
        {
            animator.SetTrigger("Skill"); // 播放技能动画
        }
    }

    /// <summary>
    /// 判断目标是否在近战攻击范围内
    /// </summary>
    private bool WithinAttackRange()
    {
        return attackTarget != null &&
            Vector3.Distance(attackTarget.transform.position, transform.position) <= enemyStats.attackData.attackRange;
    }

    /// <summary>
    /// 判断目标是否在技能攻击范围内
    /// </summary>
    private bool WithinSkillRange()
    {
        return attackTarget != null &&
            Vector3.Distance(attackTarget.transform.position, transform.position) <= enemyStats.attackData.skillRange &&
            Vector3.Distance(attackTarget.transform.position, transform.position) > enemyStats.attackData.attackRange;
    }

    /// <summary>
    /// 判断是否在视野范围内发现玩家
    /// </summary>
    private bool PlayerDetected()
    {
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);

        foreach (var target in colliders)
        {
            if (target.CompareTag("Player"))
            {
                attackTarget = target.gameObject;
                return true;
            }
        }
        attackTarget = null;
        return false;
    }

    /// <summary>
    /// 获取新的巡逻点
    /// </summary>
    void GetNewPatrolPoint()
    {
        Vector3 randomPoint = new Vector3(Random.Range(-patrolRange, patrolRange), 0, Random.Range(-patrolRange, patrolRange));
        NavMeshHit hit;
        patrolPoint = guardPosition + randomPoint;

        // 确保巡逻点在导航网格上
        if (NavMesh.SamplePosition(patrolPoint, out hit, patrolRange, 1))
        {
            patrolPoint = hit.position;
        }
        else
        {
            patrolPoint = guardPosition;
        }
    }

    /// <summary>
    /// 动画中调用该事件,触发特效或者音效
    /// </summary>
    private void Hit()
    {
        // 攻击的时候要判断玩家是否在攻击范围内
        if (attackTarget != null && transform.IsFacingTarget(attackTarget.transform))
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            targetStats.TakeDamage(enemyStats, targetStats); // 执行伤害
        }
    }

    /// <summary>
    /// 玩家死亡之后的逻辑 
    /// </summary>
    public void EndNotify()
    {
        animator.SetBool("Win", true);
        playerDead = true;
        isChasing = false;
        isFollow = false;
        attackTarget = null;
    }

    /// <summary>
    /// 绘制调试辅助信息
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRadius);
    }
}
