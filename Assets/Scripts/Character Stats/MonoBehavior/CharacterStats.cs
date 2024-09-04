using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public int previousState;

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
    /// ���㲢Ӧ���˺�
    /// </summary>
    /// <param name="attacker">������</param>
    /// <param name="defender">��������</param>
    public void TakeDamage(CharacterStats attacker, CharacterStats defender)
    {
        if (attacker == null || defender == null) return;

        int damage = CalculateDamage(attacker.CurrentDamage(), defender.CurrentDefence);
        ApplyDamage(damage, defender);

        TriggerHitAnimation(defender);
        UpdateHealthUI();
        CheckDeath(attacker, defender);
    }

    /// <summary>
    /// ���������˺�
    /// </summary>
    /// <param name="damage">��������˺�</param>
    /// <param name="defender">��������</param>
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
    /// ��������˺�ֵ�������Ǳ���
    /// </summary>
    /// <returns>�������˺�ֵ</returns>
    private int CurrentDamage()
    {
        if (attackData == null) return 0;

        float baseDamage = UnityEngine.Random.Range(attackData.minDamage, attackData.maxDamage);
        if (isCritical)
        {
            baseDamage *= attackData.criticalMultiplier;
            Debug.Log($"{name} ����� {baseDamage} �㱩���˺���");
        }
        else
        {
            Debug.Log($"{name} ����� {baseDamage} ���˺�");
        }

        return Mathf.RoundToInt(baseDamage);
    }

    /// <summary>
    /// ���������˺�ֵ
    /// </summary>
    private int CalculateDamage(int damage, int defence)
    {
        return Mathf.Max(damage - defence, 0);
    }

    /// <summary>
    /// Ӧ���˺�
    /// </summary>
    private void ApplyDamage(int damage, CharacterStats defender)
    {
        defender.CurrentHealth = Mathf.Max(defender.CurrentHealth - damage, 0);
    }

    /// <summary>
    /// ����ɫ�Ƿ�����������
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
