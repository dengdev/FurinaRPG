using UnityEngine;

public class Quest {
    public string title;          
    public string description;    
    public QuestStatus status;    
    public QuestReward reward;
    public Quest(string title, string description, QuestReward reward,QuestStatus questStatus= QuestStatus.Available) {
        this.title = title;
        this.description = description;
        this.reward = reward;
        this.status = questStatus;
    }

    public void Complete(PlayerData player) {
        status = QuestStatus.Completed;
        reward.GiveReward(player);  // 发放奖励
        Debug.Log($"任务 {title} 已完成!");
    }
}