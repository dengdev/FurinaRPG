using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DescriptionPanel : MonoBehaviour
{
    private TextMeshProUGUI description;
    private void Awake() {
        description = GetComponentInChildren<TextMeshProUGUI>();

        if (description == null) {
            Debug.LogError("DescriptionPanel: Text component not found!");
        }
    }

    private void OnEnable() {
        EventManager.Subscribe<Item>("ChangerCurrentSlot", UpdateDescription);
    }


    private void OnDisable() {
        EventManager.Unsubscribe<Item>("ChangerCurrentSlot", UpdateDescription);
    }


    public void UpdateDescription(Item item) {
        if (item != null) {
            description.text = item.description; 
        } else {
            description.text = "";
            Debug.LogWarning("当前背包中没有物品，不能更新描述");
        }
    }
}
