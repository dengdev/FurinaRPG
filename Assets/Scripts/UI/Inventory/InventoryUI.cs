using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryUI : MonoBehaviour {
    public GameObject itemSlotPrefab; // ��Ʒ��Ԥ����
    private List<Item> playerItems;

    private void OnEnable() {
        UpdateUI();
    }

    public void UpdateUI() {
        // ����ɵ���Ʒ��
        foreach (Transform child in this.transform) {
            Destroy(child.gameObject);
        }

        // ��ȡ��ҵ���Ʒ�б�
        playerItems = GameManager.Instance?.playerStats?.characterData?.items;
        Debug.Log("������ʼ��ȡ�����Ʒ�б�");

        if (playerItems != null && playerItems.Count > 0) {
            // ��̬������Ʒ��
            foreach (Item item in playerItems) {
                Debug.Log(item.item_Name);
                GameObject slot = Instantiate(itemSlotPrefab, this.transform);
                ItemSlot itemSlot = slot.GetComponent<ItemSlot>();
                itemSlot.SetItem(item);
            }
        } else {
            Debug.Log($"��ҵ�ǰû����Ʒ��������Ϊ�� {GameManager.Instance == null}");
        }
    }
}