using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Data", menuName = "Character Stats/Data")]
public class CharacterData_SO : ScriptableObject {
    [Header("Basic Stats Info")]
    public int maxHealth;
    public int currentHealth;
    public int baseDefence;
    public int currentDefence;

    [Header("Enemy Kill Exp")]
    public int killPoint;

    [Header("Player Level Up")]
    public int currentLevel;
    public int maxLevel;
    public int baseExp, currentExp;
    public float levelBuff;

    public List<Item> items; // ���ӵ�еĵ��ߺ�����
    public List<Currency> currencies; // �����б����Һ�ԭʯ

    private float LevelMultiplier { get { return 1 + (currentLevel - 1) * levelBuff; } }

    public void UpdateExp(int exp) {
        if (exp < 0) return;
        currentExp += exp;
        while (currentExp >= baseExp) {
            LevelUp();
        }
    }

    private void LevelUp() {
        currentLevel = Mathf.Clamp(currentLevel + 1, 0, maxLevel);
        currentExp -= baseExp;

        baseExp += (int)(baseExp * LevelMultiplier);
        maxHealth = (int)(maxHealth * LevelMultiplier);
        baseDefence += 1;
        RecoverPlayerState();
    }

    public void RecoverPlayerState() {
        currentHealth = maxHealth;
        currentDefence = baseDefence; 
    }
}
