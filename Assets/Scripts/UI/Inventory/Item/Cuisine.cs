using UnityEngine;

public class Cuisine : Item {
    public int healingAmount;         // жнафа©
    public int defenseBuff;
    public int effectDuration;

    public override void Use(int quality) {
        base.Use(quality);
        
        int adjustedHealing = healingAmount * quality;
        int adjustedDefense = defenseBuff * quality;

        GameManager.Instance.playerData.Heal(adjustedHealing);
        GameManager.Instance.playerData.IncreaseDefense(adjustedDefense);
    }
}

