using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class ItemSlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
    private Image icon;
    private Text quantityText;
    private Item currentItem;
    private Image quality;

    private Outline outline;
    private bool isSelected = false;

    private void Awake() {
        quality = transform.GetChild(0).transform.GetComponent<Image>();
        icon = quality.transform.GetChild(0).GetComponent<Image>();
        quantityText = transform.GetChild(1).GetComponent<Text>();

        outline = GetComponent<Outline>() ?? gameObject.AddComponent<Outline>();
        outline.effectColor = Color.white;
        outline.effectDistance = new Vector2(2, 2);
        outline.enabled = false;
    }

    public void SetItem<T>(T item) where T : Item {
        currentItem = item;
        icon.sprite = item.LoadIcon();
        UpdateInformation();
    }

    private void UpdateInformation() {
        if (currentItem != null) {
            UpdataQutity();
            quality.sprite = currentItem.LoadQuality();
        }
    }

    public void UpdataQutity() {
        if (currentItem != null) {
            quantityText.text = currentItem.Quantity.ToString();
        }
    }


    public void OnPointerClick(PointerEventData eventData) {
        FindObjectOfType<InventoryPanel>().OnSlotSelected(this);
    }

    public Item GetItem() {
        return currentItem;
    }


    public void OnPointerEnter(PointerEventData eventData) {
        if (!isSelected)
            outline.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (!isSelected)
            outline.enabled = false;
    }


    public void SetCurrentSlot() {
        outline.enabled = true;
        outline.effectColor = Color.yellow;
        isSelected = true;
    }

    public void SetDefault() {
        outline.enabled = false;
        outline.effectColor = Color.white;
        isSelected = false;
    }
}
