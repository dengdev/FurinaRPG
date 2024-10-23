using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestPanel : MonoBehaviour {

    public Transform questParent;
    public QuestDetailPanel questDetailPanel;

    private List<QuestSlot> questSlots = new();
    private HashSet<string> displayedQuests = new();
    private QuestSlot currentSelectedQuest;

    public QuestSlot questSlotPrefab;

    private void Start() {

        foreach (Quest quest in QuestManager.Instance.questList) {
            if (!displayedQuests.Contains(quest.title)) {
                CreateQuestSlot(quest);  // 仅为新任务创建任务槽
                displayedQuests.Add(quest.title);  // 标记任务已显示
            }
        }
    }

        

    public void CreateQuestSlot(Quest quest) {
        QuestSlot tmpSlot = Instantiate(questSlotPrefab, questParent);
        tmpSlot.GetComponentInChildren<Text>().text = quest.title;
        tmpSlot.GetComponentInChildren<Button>().onClick.AddListener(() => {
            questDetailPanel.DisplayQuestDetails(quest);
            SelectQuest(tmpSlot);
        });

        questSlots.Add(tmpSlot);

        if (questSlots.Count == 1) {
            SelectQuest(tmpSlot);
            questDetailPanel.DisplayQuestDetails(quest);
        }

        if (quest.status == QuestStatus.Available) {
            quest.StartQuest();
        }
    }


    private void SelectQuest(QuestSlot questSlot) {
        if (currentSelectedQuest != null && currentSelectedQuest != questSlot) {
            currentSelectedQuest.Deselect(); // 取消选中
        }
        currentSelectedQuest = questSlot; // 更新当前选中任务槽
        currentSelectedQuest.Select(); // 选中新的任务槽
    }
}