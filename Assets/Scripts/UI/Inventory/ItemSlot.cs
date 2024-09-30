using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour {
    private Image icon; // ������ʾ��Ʒͼ��
    private Item currentItem; // ��ǰ��Ʒ

    private void Awake() {
        icon = transform.GetChild(0).transform.GetComponent<Image>();
    }

    public void SetItem(Item item) {
        if (item.item_IconPath == null) {
            Debug.LogError("��ǰ��Ʒû����ͼ");
            return ;
        }
        currentItem = item;
        if (item != null) {
            icon.sprite = item.LoadIcon(); // ����ͼ��
            icon.enabled = true; // ��ʾͼ��
        } else {
            icon.sprite = null; // ���ͼ��
            icon.enabled = false; // ����ͼ��
        }
    }
}