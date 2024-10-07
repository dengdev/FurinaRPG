using UnityEngine;

[SerializeField]
public  class Item {
    public int item_ID; // ���
    public string item_Name; // ����
    public int item_MaxStack; // ��������
    public string item_IconPath; // ��ͼ�ļ�·��
    public string item_Description; // ��������
    public string item_Quality; // Ʒ��ö��
    public string item_Type; // ��Ʒ����ö��
    public int item_CurrentQuantity; // ��ǰ����

    public void Use() { } // ʹ�÷���

    public Sprite LoadIcon() {
        return Resources.Load<Sprite>($"UI/UI_Icon/{item_IconPath}");
    }
}