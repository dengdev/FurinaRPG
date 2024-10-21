using System.Collections.Generic;
using UnityEngine;

public class QuestReward {
    public int experience;  // 经验值
    public int gold;        // 金币
    public List<Item> rewardItems;  // 奖励物品列表

    public QuestReward(int exp, int goldAmount, List<Item> rewardItems = null) {
        experience = exp;
        gold = goldAmount;
        this.rewardItems = rewardItems ?? new List<Item>();
    }

    // 奖励发放
    public void GiveReward(PlayerData player) {
        player.AddExp(experience);
        //player.AddGold(gold);

        foreach (Item item in rewardItems) {
            player.AddItem(item);  
        }
    }
}
