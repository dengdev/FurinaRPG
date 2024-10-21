using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestDetailPanel : MonoBehaviour {
    public Text title;
    public Text description;
    public Transform rewardSlotParent;
    public GameObject rewardSlotPrefab;
    public TrackButton trackButton;
    private Quest currentQuest;  

    private List<Image> rewardSlots = new List<Image>();

    public void DisplayQuestDetails(Quest quest) {
        if (currentQuest != quest) {
            //trackButton.
            currentQuest = quest;
            title.text = quest.title;
            description.text = quest.description;
            UpdateRewardSlots(quest.reward.rewardItems);
        }
    }

    private void UpdateRewardSlots(List<Item> rewardItems) {
        int rewardCount = rewardItems.Count;

        for (int i = 0; i < rewardCount; i++) {
            Image slot;

            if (i >= rewardSlots.Count) {
                slot = Instantiate(rewardSlotPrefab, rewardSlotParent).GetComponent<Image>() ;
                rewardSlots.Add(slot);
            } else {
                slot = rewardSlots[i];
                slot.gameObject.SetActive(true);
            }

            slot.sprite = rewardItems[i].LoadQuality();
            slot.transform.GetChild(0).GetComponent<Image>().sprite = rewardItems[i].LoadIcon();
        }

        for (int i = rewardCount; i < rewardSlots.Count; i++) {
            rewardSlots[i].gameObject.SetActive(false);
        }
    }
}