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
            Debug.LogWarning("���û����Ʒ������Ϊ��");
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

                // ����ʹ����Ӧ��������Ʒ�ۣ�����������
                itemSlotPool.ReturnToPool(currentSlot.gameObject);
                currentSlot = null;
                SelectNextAvailableSlot();

            } else {
                currentSlot.UpdataQutity(); // ��Ʒʹ�ú����������Ϣ
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
            OnSlotSelected(itemSlots[0]); // ѡ�е�һ�����ò�
        } else {
            currentSlot = null; // û����Ʒʱ���ѡ��
        }
        EventManager.Publish<Item>("ChangerCurrentSlot", currentSlot?.GetItem()); // ˢ�����
    }

    // ����Ʒ�۱�ѡ��ʱ
    public void OnSlotSelected(ItemSlot itemSlot) {
        if (itemSlot != null) {
            currentSlot?.SetDefault();
            currentSlot = itemSlot;
            currentSlot.SetCurrentSlot();
            EventManager.Publish<Item>("ChangerCurrentSlot", itemSlot.GetItem());
        } else {
            Debug.LogWarning("Selected item is null.");
        }
    }
}