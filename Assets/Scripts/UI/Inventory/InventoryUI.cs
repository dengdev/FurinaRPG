using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryUI : MonoBehaviour {
    private List<Item> playerItems;

    private void OnEnable() {
        UpdateUI();
    }

    public void UpdateUI() {
        foreach (Transform child in this.transform) {
            Destroy(child.gameObject);
        }

        playerItems = GameManager.Instance?.playerData?.items;

        if (playerItems != null && playerItems.Count > 0) {
            foreach (Item item in playerItems) {
                GameObject slot = ResourceManager.Instance.LoadResource<GameObject>("Prefabs/UI/Inventory/ItemSlot");

                if (slot != null) {
                    // 实例化物品槽并设置物品
                    GameObject instantiatedSlot = Instantiate(slot, this.transform);
                    instantiatedSlot.GetComponent<ItemSlot>().SetItem(item);
                } else {
                    Debug.LogWarning($"物品槽预制体加载失败，路径: Prefabs/UI/Inventory/ItemSlot");
                }
            }
        } else {
            Debug.Log($"玩家当前没有物品或者数据为空 {GameManager.Instance == null}");
        }
    }
}