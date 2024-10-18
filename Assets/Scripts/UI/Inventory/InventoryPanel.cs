using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryPanel : MonoBehaviour {
    public ItemSlot currentSlot;

    private List<ItemSlot> itemSlots = new List<ItemSlot>();
    private ObjectPool itemSlotPool;

       

    private void OnEnable() {
        itemSlotPool = new ObjectPool(ResourceManager.Instance.LoadResource<GameObject>("Prefabs/UI/Inventory/ItemSlot"), 5, 20, transform);
        CanvasManager.Instance.OnInventoryToggle += UpdateInventory;
    }

    private void OnDisable() {
        CanvasManager.Instance.OnInventoryToggle -= UpdateInventory;
    }


    public void UpdateInventory() {
        List<Item> playerItems = GameManager.Instance?.playerData?.items;
        if (playerItems == null) {
            Debug.LogWarning("玩家没有物品或数据为空");
            return;
        }

        UpdateExistingSlots(playerItems);
        CreateNewSlotsIfNeeded(playerItems);
        SelectNextAvailableSlot();
    }


    public void UseCurrentSlot() {
        if (currentSlot != null) {
            Item item = currentSlot.GetItem();
            item.Use();

            if (item.IsEmpty()) {
                itemSlots.Remove(currentSlot);

                // 物体使用完应该销毁物品槽，并让面板更新
                itemSlotPool.ReturnToPool(currentSlot.gameObject);
                currentSlot = null;
                SelectNextAvailableSlot();

            } else {
                currentSlot.UpdataQutity(); // 物品使用后更新数量信息
            }
        }
    }


    private void UpdateExistingSlots(List<Item> playerItems) {
        for (int i = 0; i < playerItems.Count && i < itemSlots.Count; i++) {
            UpdateSlot(itemSlots[i], playerItems[i]);
        }
    }

    private void CreateNewSlotsIfNeeded(List<Item> playerItems) {
        for (int i = itemSlots.Count; i < playerItems.Count; i++) {
            ItemSlot newSlot = itemSlotPool.GetFromPool().GetComponent<ItemSlot>();
            UpdateSlot(newSlot, playerItems[i]);
            itemSlots.Add(newSlot);
        }
    }


    private void UpdateSlot(ItemSlot slot, Item item) {
        if (item != null) {
            slot.SetItem(item);
        } else {
            itemSlotPool.ReturnToPool(slot.gameObject);
        }
    }

    private void SelectNextAvailableSlot() {
        if (itemSlots.Count > 0) {
            OnSlotSelected(itemSlots[0]); // 选中第一个可用槽
        } else {
            currentSlot = null; // 没有物品时清空选择
        }
        EventManager.Publish("ChangerCurrentSlot", currentSlot?.GetItem()); // 刷新面板
    }

    // 当物品槽被选中时
    public void OnSlotSelected(ItemSlot itemSlot) {
        if (itemSlot != null) {
            currentSlot?.SetDefault();
            currentSlot = itemSlot;
            currentSlot.SetCurrentSlot();
            EventManager.Publish("ChangerCurrentSlot", itemSlot.GetItem());
        } else {
            Debug.LogWarning("Selected item is null.");
        }
    }
}