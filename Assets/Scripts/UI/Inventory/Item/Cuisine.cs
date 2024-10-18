using UnityEngine;

public class Cuisine : Item {
    public int healingAmount;         // 治疗量
    public int defenseBuff;
    public int effectDuration;

    public override void Use(int quality) {
        if (IsEmpty()) {
            Debug.Log($"不能使用 {itemName}，数量为零。");
            return;
        }
        base.Use(quality);
        
        int adjustedHealing = healingAmount * quality;
        int adjustedDefense = defenseBuff * quality;

        GameManager.Instance.playerData.Heal(adjustedHealing);
        GameManager.Instance.playerData.IncreaseDefense(adjustedDefense);
    }
}

