using UnityEngine;

public class Cuisine : Item {
    public int healingAmount;         // ������
    public int defenseBuff;
    public int effectDuration;

    public override void Use(int quality) {
        if (IsEmpty()) {
            Debug.Log($"����ʹ�� {itemName}������Ϊ�㡣");
            return;
        }
        base.Use(quality);
        
        int adjustedHealing = healingAmount * quality;
        int adjustedDefense = defenseBuff * quality;

        GameManager.Instance.playerData.Heal(adjustedHealing);
        GameManager.Instance.playerData.IncreaseDefense(adjustedDefense);
    }
}

