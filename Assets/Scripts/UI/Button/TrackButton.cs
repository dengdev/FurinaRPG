using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrackButton : MonoBehaviour {
    private Button trackButton;
    private TextMeshProUGUI buttonText;
    private Quest markQuest;
    private static Quest currentlyTrackedQuest;

    private void Awake() {
        trackButton = GetComponent<Button>();
        buttonText = trackButton.GetComponentInChildren<TextMeshProUGUI>();
        trackButton.onClick.AddListener(Track);
    }

    private void Track() {
        if (markQuest == null) {
            Debug.LogError("Track button clicked but no quest is assigned.");
            return;
        }

        // 如果点击的是同一个任务，切换其追踪状态
        if (markQuest == currentlyTrackedQuest) {
            markQuest.isTrack = !markQuest.isTrack;
            if (!markQuest.isTrack) {
                currentlyTrackedQuest = null; // 取消追踪
            }
        } else {
            // 取消之前的追踪任务
            if (currentlyTrackedQuest != null) {
                currentlyTrackedQuest.isTrack = false;
            }
            // 设置新任务的追踪
            markQuest.isTrack = true;
            currentlyTrackedQuest = markQuest;
        }

        // 更新按钮文本
        TrackText(markQuest);
    }

    public void SetQuest(Quest quest) {
        markQuest = quest;
        TrackText(quest); // 更新按钮文本以反映当前状态
    }

    private void TrackText(Quest quest) {
        if (quest == null) {
            Debug.LogError("TrackQuest called with a null quest object.");
            return;
        }
            buttonText.text = quest.isTrack ? "正在追踪" : "追踪任务";
    }
}
