using System;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : Singleton<CanvasManager> {
    public Dictionary<string, GameObject> canvasDict = new();
    private int openCanvasCount = 0;

    public event Action OnInventoryToggle;

    protected override void Awake() {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.B)) {
            ToggleCanvas("InventoryCanvas");
            OnInventoryToggle?.Invoke();
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
            if (isActive) {
                CloseAndDestroyCanvas(canvasName);
            } else {
                canvas.SetActive(true);
                openCanvasCount++;
            }
        } else {
            LoadAndInstantiateCanvas(canvasName);
        }
        Time.timeScale = openCanvasCount > 0 ? 0 : 1;
    }

    private void LoadAndInstantiateCanvas(string canvasName) {
        GameObject canvasPrefab = Resources.Load<GameObject>($"Prefabs/UI/Canvas/{canvasName}");
        if (canvasPrefab != null) {
            GameObject canvasInstance = Instantiate(canvasPrefab);
            canvasInstance.name = canvasPrefab.name;
            canvasInstance.SetActive(true);
            canvasDict.Add(canvasName, canvasInstance);
            openCanvasCount++;
            Debug.Log($"成功加载 Canvas: {canvasName}");
        } else {
            Debug.LogWarning($"未找到 Canvas 预制体: {canvasName}");
        }
    }

    private void CloseAndDestroyCanvas(string canvasName) {
        if (canvasDict.TryGetValue(canvasName, out GameObject canvas)) {
            canvas.SetActive(false);
            Destroy(canvas);
            canvasDict.Remove(canvasName);
            openCanvasCount = Mathf.Max(0, openCanvasCount - 1);
            Debug.Log($"成功关闭并销毁 Canvas: {canvasName}");
        }
    }
}