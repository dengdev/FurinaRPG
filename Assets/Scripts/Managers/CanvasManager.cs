using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : Singleton<CanvasManager>
{
    [SerializeField] private List<GameObject> canvases;

    protected override void Awake() {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        InitializeCanvases();
    }

    private void InitializeCanvases() {
        canvases.Clear(); // ��������б�
        GameObject[] canvasPrefabs = Resources.LoadAll<GameObject>("Canvases");
        foreach (GameObject prefab in canvasPrefabs) {
            GameObject canvasInstance = Instantiate(prefab);
            canvasInstance.SetActive(false);
            canvases.Add(canvasInstance);
        }
    }

   private  void Update() {
        // ʾ������ B ��/�رձ���
        if (Input.GetKeyDown(KeyCode.B)) {
            ToggleCanvas("InventoryCanvas");
        }

        // ʾ������ Esc ��/�رղ˵�
        if (Input.GetKeyDown(KeyCode.Escape)) {
            ToggleCanvas("MenuCanvas");
        }
    }

    public void ToggleCanvas(string canvasName) {
        GameObject canvas = canvases.Find(c => c.name == canvasName + "(Clone)");
        if (canvas != null) {
            bool isActive = canvas.activeSelf;
            canvas.SetActive(!isActive);
            Time.timeScale = isActive ? 1 : 0; // ��ͣ��ָ���Ϸ
        } else {
            Debug.Log($"δ�ҵ� Canvas: {canvasName}(Clone)");
        }
    }

     public void CloseCanvas(string canvasName) {
        GameObject canvas = canvases.Find(c => c.name == canvasName);
        if (canvas != null) {
            canvas.SetActive(false);
            Time.timeScale = 1; // �ָ���Ϸ
        }
    }
}
