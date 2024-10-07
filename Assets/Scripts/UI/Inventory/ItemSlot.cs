using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour {
    private Image icon;
    private Item currentItem;

    private void Awake() {
        icon = transform.GetChild(0).transform.GetComponent<Image>();
    }

    public void SetItem(Item item) {

        if (item.item_IconPath == null) {
            Debug.LogError("当前物品没有贴图");
            return;
        }
        currentItem = item;

        if (item != null) {
            icon.sprite = item.LoadIcon();
            icon.enabled = true;
        } else {
            icon.sprite = null;
            icon.enabled = false;
        }
    }
}