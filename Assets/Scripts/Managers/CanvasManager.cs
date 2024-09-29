using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : Singleton<CanvasManager>
{
    [SerializeField]private GameObject inventoryCanvas; // 背包 Canvas

    protected override void Awake() {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.B)) {
            ToggleInventory();
        }
    }

    public void ToggleInventory() {
        inventoryCanvas = GameObject.Find("Canvas").transform.GetChild(0).gameObject;

        if (GameManager.Instance.playerStats.characterData.items != null) {
            bool isActive = inventoryCanvas.activeSelf;
            inventoryCanvas.SetActive(!isActive);
            Time.timeScale = isActive ? 1 : 0; 
        } 
        
    }

    public void CloseInventory() {
        inventoryCanvas.SetActive(false);
        Time.timeScale = 1; // 恢复游戏
    }
    public void UpdateItemDetails(Item item) {
        // 更新 ItemDetails 文本，显示选中物品信息
        // 例如：
        // itemDetailsText.text = item.description;
    }
}
