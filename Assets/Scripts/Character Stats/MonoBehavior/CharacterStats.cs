using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    // 事件，用于更新血量条
    public event Action<int, int> UpdateHealthBarOnAttack;

    // 角色属性数据
    public CharacterData_SO templateData; // 模板数据，用于创建角色的实例数据
    public CharacterData_SO characterData; // 实例化的角色数据
    public Attackdata_SO attackData; // 角色的攻击数据

    [HideInInspector]
    public bool isCritical; // 是否触发暴击
    public int previousState;

    private Animator animator;
    private bool isHit; // 标志是否在受击动画中
    private void Awake()
    {
        // 实例化角色数据
        if (templateData != null)
            characterData = Instantiate(templateData);

        animator = GetComponent<Animator>();
    }

    #region Read from CharacterData_SO
    // 从 CharacterData_SO 读取的属性
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
    /// 计算并应用伤害
    /// </summary>
    /// <param name="attacker">攻击者</param>
    /// <param name="defender">被攻击者</param>
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
    /// 处理物体伤害
    /// </summary>
    /// <param name="damage">物体基础伤害</param>
    /// <param name="defender">被攻击者</param>
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
    /// 随机计算伤害值，并考虑暴击
    /// </summary>
    /// <returns>计算后的伤害值</returns>
    private int CurrentDamage()
    {
        if (attackData == null) return 0;

        float baseDamage = UnityEngine.Random.Range(attackData.minDamage, attackData.maxDamage);
        if (isCritical)
        {
            baseDamage *= attackData.criticalMultiplier;
            Debug.Log($"{name} 造成了 {baseDamage} 点暴击伤害！");
        }
        else
        {
            Debug.Log($"{name} 造成了 {baseDamage} 点伤害");
        }

        return Mathf.RoundToInt(baseDamage);
    }

    /// <summary>
    /// 计算最终伤害值
    /// </summary>
    private int CalculateDamage(int damage, int defence)
    {
        return Mathf.Max(damage - defence, 0);
    }

    /// <summary>
    /// 应用伤害
    /// </summary>
    private void ApplyDamage(int damage, CharacterStats defender)
    {
        defender.CurrentHealth = Mathf.Max(defender.CurrentHealth - damage, 0);
    }

    /// <summary>
    /// 检查角色是否死亡并处理
    /// </summary>
    private void CheckDeath(CharacterStats attacker, CharacterStats defender)
    {
        if (defender.CurrentHealth <= 0)
        {
            attacker.characterData.UpdateExp(characterData.killPoint);
        }
    }

    /// <summary>
    /// 触发受击动画，并在动画播放期间阻止其他行为
    /// </summary>
    private void TriggerHitAnimation(CharacterStats defender)
    {
        if (defender.isHit) return; // 如果已经在受击动画中，则不再触发

        defender.isHit = true; // 标记为正在受击

        if (defender.animator != null)
        {
            // 记录受击前的状态
            defender.previousState = defender.animator.GetCurrentAnimatorStateInfo(0).shortNameHash;

            defender.animator.SetTrigger("Hit");

            // 启动协程，等待受击动画结束后重置状态
            StartCoroutine(WaitForHitAnimation(defender));
        }
    }

    /// <summary>
    /// 协程：等待受击动画播放完毕
    /// </summary>
    private IEnumerator WaitForHitAnimation(CharacterStats defender)
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(stateInfo.length); // 等待当前动画的持续时间
       

        defender.isHit = false; // 受击动画播放完毕，允许其他行为
        // 恢复到之前的状态
        defender.animator.Play(defender.previousState);
    }

    /// <summary>
    /// 更新血量 UI
    /// </summary>
    private void UpdateHealthUI()
    {
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);
    }

    #endregion
}
