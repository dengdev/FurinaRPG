using UnityEngine;

public class Quest {
    public string title;          
    public string description;    
    public QuestStatus status;    
    public QuestReward reward;
    public int targetCount;
    public int currentCount;

    public Quest(string title, string description, int targetCount, QuestReward reward,QuestStatus questStatus= QuestStatus.Available) {
        this.title = title;
        this.description = description;
        this.targetCount = targetCount;
        this.reward = reward;
        this.status = questStatus;
    }

    public void StartQuest() {
        status = QuestStatus.InProgress;
        Debug.Log($"任务 '{title}' 已开始!");
    }

    public void UpdateProgress() {
        currentCount++;
        Debug.Log($"任务进度: {currentCount}/{targetCount}");

        if (currentCount >= targetCount) {
            Complete(GameManager.Instance.playerData);
        }
    }

    public void Complete(PlayerData player) {
        status = QuestStatus.Completed;
        reward.GiveReward(player);  // 发放奖励
        Debug.Log($"任务 {title} 已完成!");
    }
}