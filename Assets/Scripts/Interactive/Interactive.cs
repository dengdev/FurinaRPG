using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactive : MonoBehaviour
{
    public ItemName requireItem; // 可交互内容需要的物品
    public bool isDone; // 可交互内容是否完成

    public void CheckItem(ItemName itemname)
    {
        if (requireItem == itemname && !isDone)
        {
            isDone = true;
            // 使用并移除背包中的物品
            OnclickedAction();
            EventHandler.CallItemUsedEvent(itemname);
        }
    }


    /// <summary>
    /// 默认是正确的物品执行
    /// </summary>
    protected virtual void OnclickedAction()
    {

    }

    // 当玩家触发交互时调用的方法
    public virtual void Interact()
    {
        // 这里添加具体的交互逻辑，例如打开宝箱、对话、拾取物品等
        Debug.Log("物品已被交互！");
        // 示例：销毁物品
        Destroy(gameObject);
    }

}
