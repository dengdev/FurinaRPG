using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : Singleton<CanvasManager>
{
   [SerializeField] private Dictionary<string, GameObject> canvasesDict = new Dictionary<string, GameObject>();
    private int openCanvasCount = 0;

    protected override void Awake() {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        InitializeCanvases();
    }
    private void Start() {
        
    }

    private void InitializeCanvases() {
        GameObject[] canvasPrefabs = Resources.LoadAll<GameObject>("Canvases");
        canvasesDict.Clear();

        foreach (GameObject prefab in canvasPrefabs) {
            GameObject canvasInstance = Instantiate(prefab);
            canvasInstance.name = prefab.name;
            canvasInstance.SetActive(false);
            DontDestroyOnLoad(canvasInstance);
            canvasesDict.Add(prefab.name, canvasInstance);
        }
    }

   private  void Update() {
        if (Input.GetKeyDown(KeyCode.B)) {
            ToggleCanvas("InventoryCanvas");
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            ToggleCanvas("MenuCanvas");
        }
    }

    public void ToggleCanvas(string canvasName) {
        if (canvasesDict.TryGetValue(canvasName, out GameObject canvas)) {
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
        if (canvasesDict.TryGetValue(canvasName, out GameObject canvas) && canvas.activeSelf) {
            canvas.SetActive(false);
            openCanvasCount = Mathf.Max(0, openCanvasCount - 1);
            Time.timeScale = openCanvasCount > 0 ? 0 : 1;
        } else {
            Debug.LogWarning($"δ�ҵ� Canvas �� Canvas �Ѿ��ر�: {canvasName}");
        }
    }
}
