using System.Collections.Generic;
using UnityEngine;

public class QuestManager : Singleton<QuestManager> {
    public List<Quest> questList = new List<Quest>();  // 存储所有任务

    public QuestPanel questPanel;
    protected override void Awake() {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    private void Start() {

        var quest1Reward = new QuestReward(100, 50,new List<Item> { GlobalDataManager.Instance.GetItem(1) });
        Quest quest1 = new Quest("收集药草", "帮助药剂师收集10个药草", quest1Reward);

        var quest2Reward = new QuestReward(150, 100, new List<Item> { GlobalDataManager.Instance.GetItem(2) });
        Quest quest2 = new Quest("击败怪物", "击败森林中的10个怪物", quest2Reward);

        // 将任务添加到任务管理器中
        AddQuest(quest1);
        AddQuest(quest2);

        
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Q)) {
            questPanel= CanvasManager.Instance.canvasDict["QuestCanvas"].GetComponentInChildren<QuestPanel>();
            Debug.Log("任务面板已关联");
            // 动态生成任务标题
            foreach (Quest quest in questList) {
                questPanel.CreateQuestTitle(quest);
            }

        }
        if (Input.GetKeyDown(KeyCode.P)) {
            // 模拟任务完成
           Quest quest2 = GetQuest("击败怪物");
                quest2.Complete(GameManager.Instance.playerData);
        }
    }

    // 添加任务到任务列表
    public void AddQuest(Quest quest) {
        questList.Add(quest);
    }

    // 获取指定任务
    public Quest GetQuest(string title) {
        return questList.Find(q => q.title == title);
    }
}