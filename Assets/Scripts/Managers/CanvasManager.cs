using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : Singleton<CanvasManager>
{
    [SerializeField]private GameObject inventoryCanvas; // ���� Canvas

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
        Time.timeScale = 1; // �ָ���Ϸ
    }
    public void UpdateItemDetails(Item item) {
        // ���� ItemDetails �ı�����ʾѡ����Ʒ��Ϣ
        // ���磺
        // itemDetailsText.text = item.description;
    }
}
