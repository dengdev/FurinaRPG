using UnityEngine;

public class Currency {
    public string currencyName; // ��������
    public Sprite icon; // ����ͼ��
    public int quantity; // ��������
    public string description; // ����

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