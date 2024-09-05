using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ɫ����
/// </summary>
public class CharacterStats : MonoBehaviour
{
    // �¼������ڸ���Ѫ����
    public event Action<int, int> UpdateHealthBarOnAttack;

    // ��ɫ��������
    public CharacterData_SO templateData; // ģ�����ݣ����ڴ�����ɫ��ʵ������
    public CharacterData_SO characterData; // ʵ�����Ľ�ɫ����
    public Attackdata_SO attackData; // ��ɫ�Ĺ�������

    [HideInInspector]
    public bool isCritical; // �Ƿ񴥷�����
    public int previousState; // �ܻ���ָ�����һ״̬

    private Animator animator;
    private bool isHit; // ��־�Ƿ����ܻ�������
    private void Awake()
    {
        // ʵ������ɫ����
        if (templateData != null)
            characterData = Instantiate(templateData);

        animator = GetComponent<Animator>();
    }

    #region Read from CharacterData_SO
    // �� CharacterData_SO ��ȡ������
    public int MaxHealth
    {
        get { return characterData != null ? characterData.maxHealth : 0; }
        set { characterData.maxHealth = value; }
    }

    public int CurrentHealth
    {
        get { return characterData != null ? characterData.currentHealth : 0; }
        set { characterData.currentHealth = value; }
    }

    public int BaseDefence
    {
        get { return characterData != null ? characterData.baseDefence : 0; }
        set { characterData.baseDefence = value; }
    }

    public int CurrentDefence
    {
        get { return characterData != null ? characterData.currentDefence : 0; }
        set { characterData.currentDefence = value; }
    }
    #endregion

    #region Character Combat

    /// <summary>
    /// �����ɫ�˺�
    /// </summary>
    public void TakeDamage(CharacterStats attacker, CharacterStats defender)
    {
        if (attacker == null || defender == null) return;

        // 1. �ȼ��㹥��-������Ļ����˺�
        int baseDamage = CalculateDamage(attacker.CurrentDamage(), defender.CurrentDefence);

        // 2. �ڹ���-������Ļ����˺����ٽ��б����ж�
        if (isCritical)
        {
            baseDamage = (int)(baseDamage * attackData.criticalMultiplier);
            Debug.Log($"{name} ��������");
        }

        // 3. Ӧ�������˺�
        ApplyDamage(baseDamage, defender);
        // 4. �����ܻ�����
        TriggerHitAnimation(defender);
        // 5. ����Ѫ�� UI
        UpdateHealthUI();
        // 6. ����Ƿ�����
        CheckDeath(attacker, defender);
    }

    /// <summary>
    /// �����ֱ�ӹ����˺�����ʯͷ�ȣ�
    /// </summary>
    public void TakeDamage(int damage, CharacterStats defender)
    {
        if (defender == null) return;

        int currentDamage = CalculateDamage(damage, defender.CurrentDefence);
        ApplyDamage(currentDamage, defender);

        TriggerHitAnimation(defender);
        UpdateHealthUI();
        CheckDeath(GameManager.Instance.playerStats, defender);
    }

    /// <summary>
    /// ���㵱ǰ������ֱ���˺�
    /// </summary>
    private int CurrentDamage()
    {
        if (attackData == null) return 0;
        // ���ѡ���˺������ڵ�һ��ֵ
        return Mathf.RoundToInt(UnityEngine.Random.Range(attackData.minDamage, attackData.maxDamage));
    }

    /// <summary>
    /// ����-����֮����˺�
    /// </summary>
    private int CalculateDamage(int damage, int defence)
    {
        // ȷ���˺���СΪ1
        return Mathf.Max(damage - defence, 1);
    }

    /// <summary>
    ///Ӧ���˺�����֤��ɫѪ����Ϊ����
    /// </summary>
    private void ApplyDamage(int damage, CharacterStats defender)
    {
        defender.CurrentHealth = Mathf.Max(defender.CurrentHealth - damage, 0);
        Debug.Log(defender.name + "�ܵ���" + damage + "���˺�");
    }

    /// <summary>
    /// ����ɫ�Ƿ�����
    /// </summary>
    private void CheckDeath(CharacterStats attacker, CharacterStats defender)
    {
        if (defender.CurrentHealth <= 0)
        {
            attacker.characterData.UpdateExp(characterData.killPoint);
        }
    }

    /// <summary>
    /// �����ܻ����������ڶ��������ڼ���ֹ������Ϊ
    /// </summary>
    private void TriggerHitAnimation(CharacterStats defender)
    {
        if (defender.isHit) return; // ����Ѿ����ܻ������У����ٴ���

        defender.isHit = true; // ���Ϊ�����ܻ�

        if (defender.animator != null)
        {
            // ��¼�ܻ�ǰ��״̬
            defender.previousState = defender.animator.GetCurrentAnimatorStateInfo(0).shortNameHash;

            defender.animator.SetTrigger("Hit");

            // ����Э�̣��ȴ��ܻ���������������״̬
            StartCoroutine(WaitForHitAnimation(defender));
        }
    }

    /// <summary>
    /// Э�̣��ȴ��ܻ������������
    /// </summary>
    private IEnumerator WaitForHitAnimation(CharacterStats defender)
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(stateInfo.length); // �ȴ���ǰ�����ĳ���ʱ��
       

        defender.isHit = false; // �ܻ�����������ϣ�����������Ϊ
        // �ָ���֮ǰ��״̬
        defender.animator.Play(defender.previousState);
    }

    /// <summary>
    /// ����Ѫ�� UI
    /// </summary>
    private void UpdateHealthUI()
    {
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);
    }

    #endregion
}
