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
                    // ʵ������Ʒ�۲�������Ʒ
                    GameObject instantiatedSlot = Instantiate(slot, this.transform);
                    instantiatedSlot.GetComponent<ItemSlot>().SetItem(item);
                } else {
                    Debug.LogWarning($"��Ʒ��Ԥ�������ʧ�ܣ�·��: Prefabs/UI/Inventory/ItemSlot");
                }
            }
        } else {
            Debug.Log($"��ҵ�ǰû����Ʒ��������Ϊ�� {GameManager.Instance == null}");
        }
    }
}