using Unity.VisualScripting;
using UnityEngine;

public class Weapon : Item {
    public int attackPower; // ������
    private  float attackSpeed; // �����ٶ�
    private  bool isEquipped; // �Ƿ�װ��

    // ��дʹ�÷���
    public new void Use() {
        if (isEquipped) {
            Debug.Log($"ʹ������: {item_Name}��������: {attackPower}");
            // ʵ��ʹ���������߼������繥������
        } else {
            Debug.Log($"{item_Name} ��δװ����");
        }
    }

    // װ�������ķ���
    public void Equip() {
        isEquipped = true;
        Debug.Log($"װ����: {item_Name}");
    }

    // ж�������ķ���
    public void Unequip() {
        isEquipped = false;
        Debug.Log($"ж����: {item_Name}");
    }
}