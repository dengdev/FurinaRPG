using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestDetailPanel : MonoBehaviour {
    public Text title;
    public Text description;
    public Transform rewardSlotParent;
    public GameObject rewardSlotPrefab;
    public TrackButton trackButton;
    private Quest currentQuest;  // 当前正在显示的任务

    private List<Image> rewardSlots = new List<Image>();
    // 显示任务详情
    public void DisplayQuestDetails(Quest quest) {
        if (currentQuest != quest) {
            //trackButton.
            currentQuest = quest;
            title.text = quest.title;
            description.text = quest.description;
            UpdateRewardSlots(quest.reward.rewardItems);
        }
    }

    // 更新奖励槽
    private void UpdateRewardSlots(List<Item> rewardItems) {
        int rewardCount = rewardItems.Count;

        // 检查是否需要增加奖励槽
        for (int i = 0; i < rewardCount; i++) {
            Image slot;

            // 如果已有的奖励槽数量不足，则创建新的槽
            if (i >= rewardSlots.Count) {
                slot = Instantiate(rewardSlotPrefab, rewardSlotParent).GetComponent<Image>() ;
                rewardSlots.Add(slot);
            } else {
                slot = rewardSlots[i];
                slot.gameObject.SetActive(true);  // 确保槽是可见的
            }

            // 更新奖励图标
            slot.sprite = rewardItems[i].LoadIcon();
        }

        // 如果奖励槽多于实际需要的数量，隐藏多余的槽
        for (int i = rewardCount; i < rewardSlots.Count; i++) {
            rewardSlots[i].gameObject.SetActive(false);
        }
    }
}