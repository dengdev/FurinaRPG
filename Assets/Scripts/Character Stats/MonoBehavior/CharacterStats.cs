using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 角色属性
/// </summary>
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
    public int previousState; // 受击后恢复到上一状态

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
    /// 计算角色伤害
    /// </summary>
    public void TakeDamage(CharacterStats attacker, CharacterStats defender)
    {
        if (attacker == null || defender == null) return;

        // 1. 先计算攻击-防御后的基础伤害
        int baseDamage = CalculateDamage(attacker.CurrentDamage(), defender.CurrentDefence);

        // 2. 在攻击-防御后的基础伤害上再进行暴击判定
        if (isCritical)
        {
            baseDamage = (int)(baseDamage * attackData.criticalMultiplier);
            Debug.Log($"{attacker.name} 触发暴击");
        }

        // 3. 应用最终伤害
        ApplyDamage(baseDamage, defender);
        // 4. 播放受击动画
        TriggerHitAnimation(defender);
        // 5. 更新血量 UI
        UpdateHealthUI();
        // 6. 检查是否死亡
        CheckDeath(attacker, defender);
    }

    /// <summary>
    /// 方法重载，处理非直接攻击伤害（如石头等）
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
    /// 计算当前攻击的直接伤害
    /// </summary>
    private int CurrentDamage()
    {
        if (attackData == null) return 0;
        // 随机选择伤害区间内的一个值
        return Mathf.RoundToInt(UnityEngine.Random.Range(attackData.minDamage, attackData.maxDamage));
    }

    /// <summary>
    /// 攻击-防御之后的伤害，最小为1
    /// </summary>
    private int CalculateDamage(int damage, int defence)
    {
        return Mathf.Max(damage - defence, 1);
    }

    /// <summary>
    ///应用伤害，保证角色血量不为负数
    /// </summary>
    private void ApplyDamage(int damage, CharacterStats defender)
    {
        defender.CurrentHealth = Mathf.Max(defender.CurrentHealth - damage, 0);
        Debug.Log(defender.name + "受到了" + damage + "点伤害");
    }

    /// <summary>
    /// 检查角色是否死亡
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
