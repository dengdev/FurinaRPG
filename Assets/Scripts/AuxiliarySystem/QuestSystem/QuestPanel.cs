using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestPanel : MonoBehaviour {

    public GameObject questSlot; 
    public Transform questParent;  
    public QuestDetailPanel questDetailPanel;  // 任务详情面板，显示任务的详细信息

    public void CreateQuestTitle(Quest quest) {
        GameObject questTitle = Instantiate(questSlot, questParent);
        questTitle.GetComponentInChildren<Text>().text = quest.title;

        // 绑定按钮点击事件，显示任务详情
        questTitle.GetComponentInChildren<Button>().onClick.AddListener(() => {
            questDetailPanel.DisplayQuestDetails(quest);
        });
    }
}