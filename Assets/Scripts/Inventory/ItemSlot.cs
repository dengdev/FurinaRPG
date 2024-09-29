using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour {
    private Image icon; // 用于显示物品图标
    private Item currentItem; // 当前物品

    private void Awake() {
        icon = transform.GetChild(0).transform.GetComponent<Image>();
    }

    public void SetItem(Item item) {
        if (item.item_IconPath == null) {
            Debug.LogError("当前物品没有贴图");
            return ;
        }
        currentItem = item;
        if (item != null) {
            icon.sprite = item.LoadIcon(); // 设置图标
            icon.enabled = true; // 显示图标
        } else {
            icon.sprite = null; // 清空图标
            icon.enabled = false; // 隐藏图标
        }
    }
}