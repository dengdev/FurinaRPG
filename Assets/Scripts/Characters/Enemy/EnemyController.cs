using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// 确保该组件必须附加在带有 NavMeshAgent 和 CharacterStats 组件的游戏对象上
[RequireComponent(typeof(NavMeshAgent), typeof(CharacterStats))]

public class EnemyController : MonoBehaviour,IEndGameObserver
{
    private EnemyState enemyState; // 敌人的当前状态 (巡逻、追击、守卫等)
    private NavMeshAgent agent; // 用于路径导航的组件
    private Animator animator; // 控制敌人动画的组件
    private Collider coll; // 敌人的碰撞体，用于控制敌人的碰撞检测
    protected CharacterStats characterStats; // 敌人的属性数据 (生命值、攻击力等)

    [Header("Basic Settings")]
    public float sightRadius; // 敌人的视野范围
    public bool isGuard; // 判断敌人是守卫模式还是巡逻模式
    public float lookAtTime; // 敌人停止移动后，观察周围的时间
    private float speed; // 敌人的移动速度
    private float remainLookAtTime; // 剩余的观察时间
    protected GameObject attackTarget; // 当前攻击目标

    [Header("Partol State")]
    public float partolRange; // 巡逻范围
    private Vector3 wayPoint; // 巡逻的目标点
    private float lastAttackTime; // 上次攻击的时间，用于控制攻击间隔
    private Vector3 guardPos; // 守卫模式下敌人的初始位置
    private Quaternion guardRotation; // 守卫模式下敌人的初始朝向

    // 控制动画状态的布尔值
    bool isWalking, isChasing, isFollow, isDead;
    bool playerDead; // 玩家是否死亡

    void Awake()
    {
        // 初始化组件和变量
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        coll = GetComponent<Collider>();
        characterStats = GetComponent<CharacterStats>();

        speed = agent.speed; // 记录初始的移动速度
        guardPos = transform.position; // 记录守卫模式下的初始位置
        guardRotation = transform.rotation; // 记录守卫模式下的初始朝向
        remainLookAtTime = lookAtTime; // 初始化剩余观察时间
    }

    private void Start()
    {
        if (isGuard)
        {
            enemyState = EnemyState.GUARD; // 如果是守卫模式，初始化状态为守卫
        }
        else
        {
            enemyState = EnemyState.PARTOL; // 否则初始化为巡逻
            GetNewWayPoint(); // 获取新的巡逻点
        }

        // FIXME: 场景切换后可能需要重新注册观察者，这里需要优化
        GameManager.Instance.AddObserver(this);
    }

    private void OnDisable()
    {
        // 当对象被禁用时，移除观察者
        if (!GameManager.IsInitialized) return;
        GameManager.Instance.RemoveObserver(this);
    }

    private void Update()
    {
        isDead = characterStats.CurrentHealth == 0; // 判断敌人是否死亡

        if (!playerDead)
        {
            SwitchState(); // 切换敌人的状态
            lastAttackTime -= Time.deltaTime; // 更新攻击冷却时间
        }

        SwitchAnimation(); // 更新动画状态
    }

    private void SwitchAnimation()
    {
        // 设置动画参数，控制敌人动画
        animator.SetBool("Walk", isWalking);
        animator.SetBool("Chase", isChasing);
        animator.SetBool("Follow", isFollow);
        animator.SetBool("Death", isDead);
    }

    /// <summary>
    /// 根据当前的敌人状态，执行对应的逻辑
    /// </summary>
    private void SwitchState()
    {
        if (isDead)
        {
            enemyState = EnemyState.DEAD; // 如果敌人死亡，切换到死亡状态
        }
        else if (FoundPlayer())
        {
            enemyState = EnemyState.CHASE; // 如果发现玩家，切换到追击状态
        }

        switch (enemyState)
        {
            case EnemyState.GUARD:
                HandleGuardState();
                break;
            case EnemyState.PARTOL:
                HandlePartolState();
                break;
            case EnemyState.CHASE:
                HandleChaseState();
                break;
            case EnemyState.DEAD:
                HandleDeadState();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 处理守卫状态的逻辑
    /// </summary>
    private void HandleGuardState()
    {
        isChasing = false;

        if (transform.position != guardPos)
        {
            isWalking = true;
            agent.isStopped = false;
            agent.destination = guardPos;

            // 如果回到守卫位置，停止移动并恢复初始朝向
            if (Vector3.SqrMagnitude(guardPos - transform.position) <= agent.stoppingDistance)
            {
                isWalking = false;
                transform.rotation = Quaternion.Lerp(transform.rotation, guardRotation, 0.01f);
            }
        }
    }

    /// <summary>
    /// 处理巡逻状态的逻辑
    /// </summary>
    private void HandlePartolState()
    {
        isChasing = false;
        agent.speed = speed * 0.5f; // 巡逻时减慢速度

        // 判断是否到达巡逻点
        if (Vector3.Distance(wayPoint, transform.position) <= agent.stoppingDistance)
        {
            isWalking = false;
            if (remainLookAtTime > 0)
                remainLookAtTime -= Time.deltaTime; // 停留一段时间后再移动到下一个巡逻点
            else
            {
                GetNewWayPoint();
                remainLookAtTime = lookAtTime;
            }
        }
        else
        {
            isWalking = true;
            agent.destination = wayPoint;
        }
    }

    /// <summary>
    /// 处理追击状态的逻辑
    /// </summary>
    private void HandleChaseState()
    {
        isWalking = false;
        isChasing = true;

        if (!FoundPlayer())
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
                enemyState = EnemyState.PARTOL;
        }
        else
        {
            isFollow = true;
            agent.isStopped = false;
            agent.destination = attackTarget.transform.position;

            if (TargetInAttackRange() || TargetInSkillRange())
            {
                isFollow = false;
                agent.isStopped = true;

                if (lastAttackTime < 0)
                {
                    lastAttackTime = characterStats.attackData.coolDown;
                    Attack(); // 执行攻击逻辑
                }
            }
        }
    }

    /// <summary>
    /// 处理敌人死亡的逻辑
    /// </summary>
    private void HandleDeadState()
    {
        // 停止敌人的移动和交互
        agent.radius = 0;
        coll.enabled = false;
        Destroy(gameObject, 2f); // 2秒后销毁敌人对象
    }

    /// <summary>
    /// 执行敌人的攻击逻辑
    /// </summary>
    void Attack()
    {
        // 判断是否触发暴击
        characterStats.isCritical = Random.value < characterStats.attackData.criticalChance;

        // 面向攻击目标
        transform.LookAt(attackTarget.transform);

        if (TargetInAttackRange())
        {
            animator.SetTrigger("Attack"); // 播放近战攻击动画
        }

        if (TargetInSkillRange())
        {
            animator.SetTrigger("Skill"); // 播放技能攻击动画
        }
    }

    /// <summary>
    /// 判断目标是否在近战攻击范围内
    /// </summary>
    /// <returns>如果在攻击范围内返回 true，否则返回 false</returns>
    bool TargetInAttackRange()
    {
        if (attackTarget != null)
        {
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.attackRange;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 判断目标是否在技能攻击范围内
    /// </summary>
    /// <returns>如果在技能范围内返回 true，否则返回 false</returns>
    bool TargetInSkillRange()
    {
        if (attackTarget != null)
        {
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.skillRange &&
                   Vector3.Distance(attackTarget.transform.position, transform.position) > characterStats.attackData.attackRange;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 在视野范围内寻找玩家的位置
    /// </summary>
    /// <returns>如果找到玩家返回 true，否则返回 false</returns>
    bool FoundPlayer()
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
    void GetNewWayPoint()
    {
        Vector3 randomPoint = new Vector3(
            Random.Range(-partolRange, partolRange),
            0,
            Random.Range(-partolRange, partolRange)
        );

        NavMeshHit hit;
        wayPoint = guardPos + randomPoint;

        // 确保巡逻点在导航网格上
        if (NavMesh.SamplePosition(wayPoint, out hit, partolRange, 1))
        {
            wayPoint = hit.position;
        }
        else
        {
            wayPoint = guardPos;
        }
    }

    /// <summary>
    /// 动画中调用该事件
    /// </summary>
    private void Hit()
    {
        if(attackTarget != null&&transform.IsFacingTarget(attackTarget.transform))
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();

            targetStats.TakeDamage(characterStats, targetStats);
        }
        
    }

    /// <summary>
    /// 用于场景切换时的清理工作
    /// </summary>
    public void EndNotify()
    {
        // 场景切换时清除目标，重置敌人状态
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
