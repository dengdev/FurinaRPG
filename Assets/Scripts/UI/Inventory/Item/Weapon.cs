using Unity.VisualScripting;
using UnityEngine;

public class Weapon : Item {
    public int attackPower; // ������
    private  float attackSpeed; // �����ٶ�
    private  bool isEquipped; // �Ƿ�װ��

    // ��дʹ�÷���
    public  void Use() {
        if (isEquipped) {
            Debug.Log($"ʹ������: {itemName}��������: {attackPower}");
            // ʵ��ʹ���������߼������繥������
        } else {
            Debug.Log($"{itemName} ��δװ����");
        }
    }

    // װ�������ķ���
    public void Equip() {
        isEquipped = true;
        Debug.Log($"װ����: {itemName}");
    }

    // ж�������ķ���
    public void Unequip() {
        isEquipped = false;
        Debug.Log($"ж����: {itemName}");
    }
}