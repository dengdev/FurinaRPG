using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// ȷ����������븽���ڴ��� NavMeshAgent �� CharacterStats �������Ϸ������
[RequireComponent(typeof(NavMeshAgent), typeof(CharacterStats))]

public class EnemyController : MonoBehaviour,IEndGameObserver
{
    private EnemyState enemyState; // ���˵ĵ�ǰ״̬ (Ѳ�ߡ�׷����������)
    private NavMeshAgent agent; // ����·�����������
    private Animator animator; // ���Ƶ��˶��������
    private Collider coll; // ���˵���ײ�壬���ڿ��Ƶ��˵���ײ���
    protected CharacterStats characterStats; // ���˵��������� (����ֵ����������)

    [Header("Basic Settings")]
    public float sightRadius; // ���˵���Ұ��Χ
    public bool isGuard; // �жϵ���������ģʽ����Ѳ��ģʽ
    public float lookAtTime; // ����ֹͣ�ƶ��󣬹۲���Χ��ʱ��
    private float speed; // ���˵��ƶ��ٶ�
    private float remainLookAtTime; // ʣ��Ĺ۲�ʱ��
    protected GameObject attackTarget; // ��ǰ����Ŀ��

    [Header("Partol State")]
    public float partolRange; // Ѳ�߷�Χ
    private Vector3 wayPoint; // Ѳ�ߵ�Ŀ���
    private float lastAttackTime; // �ϴι�����ʱ�䣬���ڿ��ƹ������
    private Vector3 guardPos; // ����ģʽ�µ��˵ĳ�ʼλ��
    private Quaternion guardRotation; // ����ģʽ�µ��˵ĳ�ʼ����

    // ���ƶ���״̬�Ĳ���ֵ
    bool isWalking, isChasing, isFollow, isDead;
    bool playerDead; // ����Ƿ�����

    void Awake()
    {
        // ��ʼ������ͱ���
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        coll = GetComponent<Collider>();
        characterStats = GetComponent<CharacterStats>();

        speed = agent.speed; // ��¼��ʼ���ƶ��ٶ�
        guardPos = transform.position; // ��¼����ģʽ�µĳ�ʼλ��
        guardRotation = transform.rotation; // ��¼����ģʽ�µĳ�ʼ����
        remainLookAtTime = lookAtTime; // ��ʼ��ʣ��۲�ʱ��
    }

    private void Start()
    {
        if (isGuard)
        {
            enemyState = EnemyState.GUARD; // ���������ģʽ����ʼ��״̬Ϊ����
        }
        else
        {
            enemyState = EnemyState.PARTOL; // �����ʼ��ΪѲ��
            GetNewWayPoint(); // ��ȡ�µ�Ѳ�ߵ�
        }

        // FIXME: �����л��������Ҫ����ע��۲��ߣ�������Ҫ�Ż�
        GameManager.Instance.AddObserver(this);
    }

    private void OnDisable()
    {
        // �����󱻽���ʱ���Ƴ��۲���
        if (!GameManager.IsInitialized) return;
        GameManager.Instance.RemoveObserver(this);
    }

    private void Update()
    {
        isDead = characterStats.CurrentHealth == 0; // �жϵ����Ƿ�����

        if (!playerDead)
        {
            SwitchState(); // �л����˵�״̬
            lastAttackTime -= Time.deltaTime; // ���¹�����ȴʱ��
        }

        SwitchAnimation(); // ���¶���״̬
    }

    private void SwitchAnimation()
    {
        // ���ö������������Ƶ��˶���
        animator.SetBool("Walk", isWalking);
        animator.SetBool("Chase", isChasing);
        animator.SetBool("Follow", isFollow);
        animator.SetBool("Death", isDead);
    }

    /// <summary>
    /// ���ݵ�ǰ�ĵ���״̬��ִ�ж�Ӧ���߼�
    /// </summary>
    private void SwitchState()
    {
        if (isDead)
        {
            enemyState = EnemyState.DEAD; // ��������������л�������״̬
        }
        else if (FoundPlayer())
        {
            enemyState = EnemyState.CHASE; // ���������ң��л���׷��״̬
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
    /// ��������״̬���߼�
    /// </summary>
    private void HandleGuardState()
    {
        isChasing = false;

        if (transform.position != guardPos)
        {
            isWalking = true;
            agent.isStopped = false;
            agent.destination = guardPos;

            // ����ص�����λ�ã�ֹͣ�ƶ����ָ���ʼ����
            if (Vector3.SqrMagnitude(guardPos - transform.position) <= agent.stoppingDistance)
            {
                isWalking = false;
                transform.rotation = Quaternion.Lerp(transform.rotation, guardRotation, 0.01f);
            }
        }
    }

    /// <summary>
    /// ����Ѳ��״̬���߼�
    /// </summary>
    private void HandlePartolState()
    {
        isChasing = false;
        agent.speed = speed * 0.5f; // Ѳ��ʱ�����ٶ�

        // �ж��Ƿ񵽴�Ѳ�ߵ�
        if (Vector3.Distance(wayPoint, transform.position) <= agent.stoppingDistance)
        {
            isWalking = false;
            if (remainLookAtTime > 0)
                remainLookAtTime -= Time.deltaTime; // ͣ��һ��ʱ������ƶ�����һ��Ѳ�ߵ�
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
    /// ����׷��״̬���߼�
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
                    Attack(); // ִ�й����߼�
                }
            }
        }
    }

    /// <summary>
    /// ��������������߼�
    /// </summary>
    private void HandleDeadState()
    {
        // ֹͣ���˵��ƶ��ͽ���
        agent.radius = 0;
        coll.enabled = false;
        Destroy(gameObject, 2f); // 2������ٵ��˶���
    }

    /// <summary>
    /// ִ�е��˵Ĺ����߼�
    /// </summary>
    void Attack()
    {
        // �ж��Ƿ񴥷�����
        characterStats.isCritical = Random.value < characterStats.attackData.criticalChance;

        // ���򹥻�Ŀ��
        transform.LookAt(attackTarget.transform);

        if (TargetInAttackRange())
        {
            animator.SetTrigger("Attack"); // ���Ž�ս��������
        }

        if (TargetInSkillRange())
        {
            animator.SetTrigger("Skill"); // ���ż��ܹ�������
        }
    }

    /// <summary>
    /// �ж�Ŀ���Ƿ��ڽ�ս������Χ��
    /// </summary>
    /// <returns>����ڹ�����Χ�ڷ��� true�����򷵻� false</returns>
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
    /// �ж�Ŀ���Ƿ��ڼ��ܹ�����Χ��
    /// </summary>
    /// <returns>����ڼ��ܷ�Χ�ڷ��� true�����򷵻� false</returns>
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
    /// ����Ұ��Χ��Ѱ����ҵ�λ��
    /// </summary>
    /// <returns>����ҵ���ҷ��� true�����򷵻� false</returns>
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
    /// ��ȡ�µ�Ѳ�ߵ�
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

        // ȷ��Ѳ�ߵ��ڵ���������
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
    /// �����е��ø��¼�
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
    /// ���ڳ����л�ʱ��������
    /// </summary>
    public void EndNotify()
    {
        // �����л�ʱ���Ŀ�꣬���õ���״̬
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
