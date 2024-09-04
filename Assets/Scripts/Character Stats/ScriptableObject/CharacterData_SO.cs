using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Data",menuName ="Character Stats/Data")]
public class CharacterData_SO : ScriptableObject
{
    [Header("Stats Info")]
    public int maxHealth; // 角色的最大生命值
    public int currentHealth; // 角色的当前生命值
    public int baseDefence; // 角色的基础防御值
    public int currentDefence; // 角色的当前防御值

    [Header("Kill Point")]
    public int killPoint; // 角色的击杀点数，用于计算经验或者其他目的

    [Header("Level Up")] // 角色升级相关的属性，仅对玩家角色生效
    public int currentLevel; // 角色当前等级
    public int maxLevel; // 角色的最高等级
    public int baseExp, currentExp; // 角色升级所需的基础经验值和当前经验值
    public float levelBuff; // 每次升级时，属性的百分比增长倍率

    /// <summary>
    /// 更新角色的经验值，并检测是否达到升级条件
    /// </summary>
    public void UpdateExp(int point)
    {
        currentExp += point; // 增加经验值
        if (currentExp >= baseExp) // 如果当前经验值达到或超过所需经验值
            LevelUp(); // 调用升级方法
    }

    /// <summary>
    /// 获取当前等级的属性提升倍率
    /// </summary>
    float LevelMultiplier
    {
        get { return 1 + (currentLevel - 1) * levelBuff; } // 计算当前等级的属性提升倍率
    }

    /// <summary>
    /// 玩家等级提升，基础属性提升逻辑
    /// </summary>
    private void LevelUp()
    {
        currentLevel = Mathf.Clamp(currentLevel + 1, 0, maxLevel); // 等级提升1级，并且不超过最大等级
        baseExp += (int)(baseExp * LevelMultiplier); // 根据等级倍率增加升级所需经验值

        maxHealth = (int)(maxHealth * LevelMultiplier); // 根据等级倍率增加最大生命值
        baseDefence += 1; // 每次升级基础防御值增加1点

        currentHealth = maxHealth; // 升级后将当前生命值恢复到最大生命值
        currentDefence = baseDefence; // 升级后将当前防御值恢复到基础防御值
    }
}
