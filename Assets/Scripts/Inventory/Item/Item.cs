using UnityEngine;

public  class Item {
    public int item_ID; // 序号
    public string item_Name; // 名字
    public int item_MaxStack; // 数量上限
    public string item_IconPath; // 贴图文件路径
    public string item_Description; // 文字描述
    public string item_Quality; // 品质枚举
    public string item_Type; // 物品种类枚举
    public int item_CurrentQuantity; // 当前数量

    public void Use() { } // 使用方法

    public Sprite LoadIcon() {
        return Resources.Load<Sprite>($"UI/UI_Icon/{item_IconPath}");
    }
}