using UnityEngine;

public class Currency {
    public string currencyName; // 货币名称
    public Sprite icon; // 货币图标
    public int quantity; // 货币数量
    public string description; // 描述

    public Currency(string name, Sprite icon, int quantity, string description) {
        this.currencyName = name;
        this.icon = icon;
        this.quantity = quantity;
        this.description = description;
    }

    public void Add(int amount) {
        quantity += amount;
    }

    public bool Remove(int amount) {
        if (quantity >= amount) {
            quantity -= amount;
            return true;
        }
        return false;
    }
}