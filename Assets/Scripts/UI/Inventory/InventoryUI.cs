using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryUI : MonoBehaviour {
    public GameObject itemSlotPrefab; // 物品槽预制体
    private List<Item> playerItems;

    private void OnEnable() {
        UpdateUI();
    }

    public void UpdateUI() {
        // 清除旧的物品槽
        foreach (Transform child in this.transform) {
            Destroy(child.gameObject);
        }

        // 获取玩家的物品列表
        playerItems = GameManager.Instance?.playerStats?.characterData?.items;
        Debug.Log("背包开始获取玩家物品列表");

        if (playerItems != null && playerItems.Count > 0) {
            // 动态生成物品槽
            foreach (Item item in playerItems) {
                Debug.Log(item.item_Name);
                GameObject slot = Instantiate(itemSlotPrefab, this.transform);
                ItemSlot itemSlot = slot.GetComponent<ItemSlot>();
                itemSlot.SetItem(item);
            }
        } else {
            Debug.Log($"玩家当前没有物品或者数据为空 {GameManager.Instance == null}");
        }
    }
}