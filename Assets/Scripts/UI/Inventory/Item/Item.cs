using UnityEngine;

public class Item {
    public int itemId;
    public string itemName;
    public int maxStack;
    public string iconPath;
    public string description;
    public string quality;
    public string type;

    private int quantity;

    public int Quantity {
        get => quantity;
        set {
            quantity = Mathf.Clamp(value, 0, maxStack);
        }
    }

    public virtual void Use(int quantity = 1) {

        if (Quantity >= quantity) {
            Quantity -= quantity;
            if (Quantity <= 0) {
                GameManager.Instance.playerData.items.Remove(this);
            }
            Debug.Log($"ʹ����Ʒ: {itemName}, ʣ������: {Quantity}");
        }
    }

    public bool IsEmpty() {
        return Quantity <= 0;
    }

    public Sprite LoadIcon() {
        return Resources.Load<Sprite>($"UI/UI_Icon/{iconPath}");
    }

    public Sprite LoadQuality() {
        return ResourceManager.Instance.LoadResource<Sprite>($"UI/UI_Icon/SlotUI/{quality}");
    }
}