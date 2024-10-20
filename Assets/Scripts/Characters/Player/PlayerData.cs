using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : Characters {

    public int currentLevel;
    public int maxLevel;
    public int baseExp, currentExp;
    public float levelBuff;
    public List<Item> items;
    //public List<Currency> currencies;

    // PlayerData ���캯����������������
    public PlayerData(int currentLevel, int maxLevel, int baseExp, int currentExp, float levelBuff,
                      int maxHealth, int currentHealth, int baseDefence, int currentDefence , List<Item> items)
        : base(maxHealth, currentHealth, baseDefence, currentDefence) // ���ø��๹�캯��
    {
        this.currentLevel = currentLevel;
        this.maxLevel = maxLevel;
        this.baseExp = baseExp;
        this.currentExp = currentExp;
        this.levelBuff = levelBuff;
        this.items =  items;
    }

    private float LevelMultiplier { get { return 1 + (currentLevel - 1) * levelBuff; } }

    public void AddExp(int exp) {
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

    public void AddItem(Item item, int quality = 1) {
        Item existingItem = items.Find(i => i.itemId  == item.itemId);
        if (existingItem != null) {
            existingItem.Quantity += quality;
        } else {
            item.Quantity = quality;
            items.Add(item);
        }
    }
    public void Heal(int amount) {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        Debug.Log($"��һָ��� {amount} ����ֵ����ǰ����ֵ: {currentHealth}");
    }


    public void IncreaseDefense(int amount) {
        currentDefence += amount;
        Debug.Log($"��ҷ��������� {amount}����ǰ������: {currentDefence}");
    }

}
