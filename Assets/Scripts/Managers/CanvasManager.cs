using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : Singleton<CanvasManager> {

    public Dictionary<string, GameObject> canvasDict = new();
    private int openCanvasCount = 0;

    public event Action OnInventoryToggle;

    protected override void Awake() {
        base.Awake();
        InitializeCanvases();
    }

    private void InitializeCanvases() {
        GameObject[] canvasPrefabs = Resources.LoadAll<GameObject>("Prefabs/Canvas");
        canvasDict.Clear();

        foreach (GameObject prefab in canvasPrefabs) {
            GameObject canvasInstance = Instantiate(prefab);
            
            canvasInstance.name = prefab.name;
            canvasInstance.SetActive(false);
            canvasDict.Add(prefab.name, canvasInstance);
        }
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.B)) {
            ToggleCanvas("InventoryCanvas");
            OnInventoryToggle?.Invoke(); // ����岢����
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            ToggleCanvas("MenuCanvas");
        }
        if (Input.GetKeyDown(KeyCode.Tab)) {
            ToggleCanvas("QuestCanvas");
        }
    }

    public void ToggleCanvas(string canvasName) {
        if (canvasDict.TryGetValue(canvasName, out GameObject canvas)) {
            bool isActive = canvas.activeSelf;
            canvas.SetActive(!isActive);

            if (isActive) {
                openCanvasCount = Mathf.Max(0, openCanvasCount - 1);
            } else {
                openCanvasCount++;
            }

            // ������κλ����򿪣���ͣ��Ϸ������ָ���Ϸ
            Time.timeScale = openCanvasCount > 0 ? 0 : 1;
        } else {
            Debug.LogWarning($"δ�ҵ� Canvas: {canvasName}");
        }
    }

    public void CloseCanvas(string canvasName) {
        if (canvasDict.TryGetValue(canvasName, out GameObject canvas) && canvas.activeSelf) {
            canvas.SetActive(false);
            openCanvasCount = Mathf.Max(0, openCanvasCount - 1);
            Time.timeScale = openCanvasCount > 0 ? 0 : 1;
        } else {
            Debug.LogWarning($"δ�ҵ� Canvas �� Canvas �Ѿ��ر�: {canvasName}");
        }
    }
}
