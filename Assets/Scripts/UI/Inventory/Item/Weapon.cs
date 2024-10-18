using Unity.VisualScripting;
using UnityEngine;

public class Weapon : Item {
    public int attackPower; // 攻击力
    private  float attackSpeed; // 攻击速度
    private  bool isEquipped; // 是否被装备

    // 重写使用方法
    public  void Use() {
        if (isEquipped) {
            Debug.Log($"使用武器: {itemName}，攻击力: {attackPower}");
            // 实现使用武器的逻辑，例如攻击敌人
        } else {
            Debug.Log($"{itemName} 尚未装备。");
        }
    }

    // 装备武器的方法
    public void Equip() {
        isEquipped = true;
        Debug.Log($"装备了: {itemName}");
    }

    // 卸下武器的方法
    public void Unequip() {
        isEquipped = false;
        Debug.Log($"卸下了: {itemName}");
    }
}