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

    // PlayerData 构造函数，包含父类属性
    public PlayerData(int currentLevel, int maxLevel, int baseExp, int currentExp, float levelBuff,
                      int maxHealth, int currentHealth, int baseDefence, int currentDefence , List<Item> items)
        : base(maxHealth, currentHealth, baseDefence, currentDefence) // 调用父类构造函数
    {
        this.currentLevel = currentLevel;
        this.maxLevel = maxLevel;
        this.baseExp = baseExp;
        this.currentExp = currentExp;
        this.levelBuff = levelBuff;
        this.items =  items;
    }

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
        Debug.Log($"玩家恢复了 {amount} 生命值，当前生命值: {currentHealth}");
    }


    public void IncreaseDefense(int amount) {
        currentDefence += amount;
        Debug.Log($"玩家防御力增加 {amount}，当前防御力: {currentDefence}");
    }

}
