using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Data",menuName ="Character Stats/Data")]
public class CharacterData_SO : ScriptableObject
{
    [Header("Stats Info")]
    public int maxHealth; 
    public int currentHealth; 
    public int baseDefence; 
    public int currentDefence;

    [Header("Kill Point")]
    public int killPoint; 

    [Header("Level Up")]
    public int currentLevel; 
    public int maxLevel;
    public int baseExp, currentExp; 
    public float levelBuff; 

    public void UpdateExp(int point)
    {
        currentExp += point; // 增加经验值
        if (currentExp >= baseExp) // 如果当前经验值达到或超过所需经验值
            LevelUp(); // 调用升级方法
    }

    float LevelMultiplier
    {
        get { return 1 + (currentLevel - 1) * levelBuff; } // 计算当前等级的属性提升倍率
    }

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
