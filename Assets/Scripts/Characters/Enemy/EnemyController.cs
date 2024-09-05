using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// ȷ������������ھ��� NavMeshAgent �� CharacterStats ����Ķ�����
[RequireComponent(typeof(NavMeshAgent), typeof(CharacterStats))]

public class EnemyController : MonoBehaviour, IEndGameObserver
{
    private EnemyState enemyState; // ���˵ĵ�ǰ״̬ (Ѳ�ߡ�׷����������)
    private NavMeshAgent agent; // ����·�����������
    private Animator animator; // ���Ƶ��˶��������
    private Collider coll; // ���˵���ײ�壬���ڼ����ײ
    protected CharacterStats enemyStats; // ���˵��������� (����ֵ����������)

    [Header("Basic Settings")]
    public float sightRadius; // ���˵���Ұ��Χ
    public bool isGuard; // �жϵ���������ģʽ����Ѳ��ģʽ
    public float lookAtTime; // ����ֹͣ�ƶ���۲컷����ʱ��
    private float movementSpeed; // ���˵��ƶ��ٶ�
    private float remainLookAtTime; // ʣ��Ĺ۲�ʱ��
    protected GameObject attackTarget; // ��ǰ����Ŀ��

    [Header("Partol State")]
    public float patrolRange; // Ѳ�߷�Χ
    private Vector3 patrolPoint; // Ѳ�ߵ�Ŀ���
    private float lastAttackTime; // �ϴι�����ʱ�䣬���ڿ��ƹ������
    private Vector3 guardPosition; // ����ģʽ�µ��˵ĳ�ʼλ��
    private Quaternion guardRotation; // ����ģʽ�µ��˵ĳ�ʼ����

    [Header("Alert Settings")]
    public float alertTime = 2f; // ����ʱ��
    private float alertTimer; // ��ǰ�ľ�����ʱ��

    // ���ƶ���״̬�Ĳ���ֵ
    bool isWalking, isChasing, isFollow, isDead;
    bool playerDead; // ����Ƿ�����

    // ��ʼ������ͱ���
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        coll = GetComponent<Collider>();
        enemyStats = GetComponent<CharacterStats>();

        movementSpeed = agent.speed; // ��¼��ʼ�ƶ��ٶ�
        guardPosition = transform.position; // ��¼������ʼλ��
        guardRotation = transform.rotation; // ��¼������ʼ����
        remainLookAtTime = lookAtTime; // ��ʼ���۲�ʱ��
    }

    // ��ʼʱ���õ���״̬
    private void Start()
    {
        enemyState = isGuard ? EnemyState.GUARD : EnemyState.PATROL; // ��ʼ��״̬
        if (!isGuard)
        {
            GetNewPatrolPoint(); // ��ȡѲ�ߵ�
        }

        // FIXME: �����л��������Ҫ����ע��۲��ߣ�������Ҫ�Ż�
        GameManager.Instance.AddObserver(this);
    }

    // �������ʱ�Ƴ��۲���
    private void OnDisable()
    {
        if (!GameManager.IsInitialized) return;
        GameManager.Instance.RemoveObserver(this);
    }

    private void Update()
    {
        isDead = enemyStats.CurrentHealth == 0; // �жϵ����Ƿ�����

        if (!playerDead)
        {
            UpdateState(); // �л����˵�״̬
            lastAttackTime -= Time.deltaTime; // ���¹�����ȴ
        }

        UpdateAnimation(); // ���¶���״̬
    }

    private void UpdateAnimation()
    {
        // ���ö������������Ƶ��˶���
        animator.SetBool("Walk", isWalking);
        animator.SetBool("Chase", isChasing);
        animator.SetBool("Follow", isFollow);
        animator.SetBool("Death", isDead);
    }

    /// <summary>
    /// ���ݵ���״ִ̬�ж�Ӧ���߼�
    /// </summary>
    private void UpdateState()
    {
        if (isDead)
        {   // ��������������л�������״̬
            enemyState = EnemyState.DEAD;
        }
        else if (PlayerDetected())
        {
            if (enemyState != EnemyState.ALERT && enemyState != EnemyState.CHASE) // ������Һ���뾯��״̬
            {
                Debug.Log(this.name + "�������");
                transform.LookAt(attackTarget.transform);

                enemyState = EnemyState.ALERT;
                alertTimer = alertTime; // ���þ�����ʱ��
            }
        }
        else if (enemyState == EnemyState.ALERT)
        {
            // ������������Ұ��Χ���ָ���Ĭ��״̬
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

        // ����ʱ�䵹��ʱ
        alertTimer -= Time.deltaTime;
        if (alertTimer <= 0)
        {
            enemyState = EnemyState.CHASE; // ����ʱ��������л���׷��״̬
        }
        else
        {
            // ����״̬�¿���ѡ�����ԭ�صȴ������������
            agent.destination = transform.position; // ���ֵ�ǰվλ
        }
    }

    /// <summary>
    /// ����״̬���߼�
    /// </summary>
    private void ExecuteGuardBehavior()
    {
        isChasing = false;

        if (transform.position != guardPosition)
        {
            isWalking = true;
            agent.isStopped = false;
            agent.destination = guardPosition;

            // ����ص�����λ�ã�ֹͣ�ƶ����ָ���ʼ����
            if (Vector3.SqrMagnitude(guardPosition - transform.position) <= agent.stoppingDistance)
            {
                isWalking = false;
                transform.rotation = Quaternion.Lerp(transform.rotation, guardRotation, 0.01f);
            }
        }
    }

    /// <summary>
    /// Ѳ��״̬���߼�
    /// </summary>
    private void ExecutePatrolBehavior()
    {
        isChasing = false;
        agent.speed = movementSpeed * 0.5f; // Ѳ��ʱ�����ٶ�

        // �ж��Ƿ񵽴�Ѳ�ߵ�
        if (Vector3.Distance(patrolPoint, transform.position) <= agent.stoppingDistance)
        {
            isWalking = false;
            if (remainLookAtTime > 0)
                remainLookAtTime -= Time.deltaTime; // ͣ��һ��ʱ������ƶ�����һ��Ѳ�ߵ�
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
    /// ׷��ʱ�����ݾ��봥���������߼���
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
        {   // ������һ����Ӧʱ��
            isFollow = true;
            agent.isStopped = false;
            agent.destination = attackTarget.transform.position;

            if (WithinAttackRange() || WithinSkillRange())
            {
                isFollow = false;
                agent.isStopped = true;
                if (lastAttackTime < 0)
                {   // ���Թ���
                    lastAttackTime = enemyStats.attackData.coolDown;
                    PerformAttack(); // ִ�й����߼�
                }
            }
        }
    }

    /// <summary>
    /// ִ�е����������߼�
    /// </summary>
    private void ExecuteDeadBehavior()
    {
        agent.radius = 0;
        coll.enabled = false;
        Destroy(gameObject, 2f); // 2������ٵ��˶���
    }

    /// <summary>
    /// ִ�е��˹���
    /// </summary>
    void PerformAttack()
    {
        // �ж��Ƿ񴥷�����
        enemyStats.isCritical = Random.value < enemyStats.attackData.criticalChance;

        // ���򹥻�Ŀ��
        transform.LookAt(attackTarget.transform);

        if (WithinAttackRange())
        {
            animator.SetTrigger("Attack"); // ���Ź�������
        }

        if (WithinSkillRange())
        {
            animator.SetTrigger("Skill"); // ���ż��ܶ���
        }
    }

    /// <summary>
    /// �ж�Ŀ���Ƿ��ڽ�ս������Χ��
    /// </summary>
    private bool WithinAttackRange()
    {
        return attackTarget != null &&
            Vector3.Distance(attackTarget.transform.position, transform.position) <= enemyStats.attackData.attackRange;
    }

    /// <summary>
    /// �ж�Ŀ���Ƿ��ڼ��ܹ�����Χ��
    /// </summary>
    private bool WithinSkillRange()
    {
        return attackTarget != null &&
            Vector3.Distance(attackTarget.transform.position, transform.position) <= enemyStats.attackData.skillRange &&
            Vector3.Distance(attackTarget.transform.position, transform.position) > enemyStats.attackData.attackRange;
    }

    /// <summary>
    /// �ж��Ƿ�����Ұ��Χ�ڷ������
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
    /// ��ȡ�µ�Ѳ�ߵ�
    /// </summary>
    void GetNewPatrolPoint()
    {
        Vector3 randomPoint = new Vector3(Random.Range(-patrolRange, patrolRange), 0, Random.Range(-patrolRange, patrolRange));
        NavMeshHit hit;
        patrolPoint = guardPosition + randomPoint;

        // ȷ��Ѳ�ߵ��ڵ���������
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
    /// �����е��ø��¼�,������Ч������Ч
    /// </summary>
    private void Hit()
    {
        // ������ʱ��Ҫ�ж�����Ƿ��ڹ�����Χ��
        if (attackTarget != null && transform.IsFacingTarget(attackTarget.transform))
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            targetStats.TakeDamage(enemyStats, targetStats); // ִ���˺�
        }
    }

    /// <summary>
    /// �������л�ʱ���ã�����״̬
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
    /// ���Ƶ��Ը�����Ϣ
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRadius);
    }
}
