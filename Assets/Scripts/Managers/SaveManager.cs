using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System;
using System.Text;

public class ItemList {
    public List<Item> items;
}

public class SaveManager : Singleton<SaveManager> {
    public Dictionary<int, Item> allItems; // 使用字典存储全局物品列表

    private string jsonFolder;
    private List<ISaveable> saveableList = new List<ISaveable>();
    private Dictionary<string, GameSaveData> saveDataDictionary = new Dictionary<string, GameSaveData>();


    protected override void Awake() {
        base.Awake();
        jsonFolder = Application.persistentDataPath + "/GameSaveData/";
        DontDestroyOnLoad(this.gameObject);
    }
    private void OnEnable() {

    }

    private void Start() {
        LoadAllItems(); // 在游戏开始时加载所有物品信息


    }
    private void OnDisable() {

    }

    public void OnStartNewGame() {
        string resultPath = jsonFolder + "data.sav";

        if (File.Exists(resultPath)) {
            File.Delete(resultPath);
        }
    }

    public void RegisterSaveable(ISaveable saveable) {
        //要确保唯一;
        saveableList.Add(saveable);
    }

    /// <summary>
    /// 保存特定的数据
    /// </summary>
    public void Save() {
        try {
            saveDataDictionary.Clear();
            foreach (ISaveable saveable in saveableList) {
                saveDataDictionary.Add(saveable.GetType().Name, saveable.GenerateSaveData());
            }
            string resultPath = jsonFolder + "data.sav";

            string jsonData = JsonConvert.SerializeObject(saveDataDictionary, Formatting.Indented);

            if (!File.Exists(resultPath)) {
                Directory.CreateDirectory(jsonFolder);
            }
            File.WriteAllText(resultPath, jsonData);
        } catch (Exception ex) {
            Debug.LogError($"保存失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 从已保存的数据中加载
    /// </summary>
    public void Load() {
        try {
            string resultPath = jsonFolder + "data.sav";

            if (!File.Exists(resultPath)) { return; }

            string stringData = File.ReadAllText(resultPath);

            Dictionary<string, GameSaveData> jsonData = JsonConvert.DeserializeObject<Dictionary<string, GameSaveData>>(stringData);

            foreach (ISaveable saveable in saveableList) {
                saveable.RestoreGameData(jsonData[saveable.GetType().Name]);
            }
        } catch (Exception ex) {
            Debug.LogError($"加载失败: {ex.Message}");
        }
    }





    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Debug.Log("按下Esc返回到主菜单");
            SceneController.Instance.TransitionToMainMenuScene();
        }

        if (Input.GetKeyDown(KeyCode.R)) {

            Debug.Log("按下R保存玩家数据");
            Save();
        }

        if (Input.GetKeyDown(KeyCode.L)) {

            Debug.Log("按下L读取玩家数据");
            Load();
        }
    }

    private void LoadAllItems() {
        string path = Path.Combine(Application.dataPath, "Resources/Data/items.json");
        if (File.Exists(path)) {

            string stringData = File.ReadAllText(path, Encoding.UTF8);
            Debug.Log(stringData);
            ItemList itemList = JsonConvert.DeserializeObject<ItemList>(stringData, new ItemConverter());
            allItems = new Dictionary<int, Item>();

            foreach (Item item in itemList.items) {
                allItems[item.item_ID] = item;
            }
            Debug.Log("所有物品数据加载成功。");
        } else {
            Debug.LogError("未能找到 JSON 文件: " + path);
        }
    }

    public Item GetItem(int itemID) {
        allItems.TryGetValue(itemID, out Item item);
        if (item != null) {
            return item;
        } else {
            Debug.Log($"此{itemID}为无效ID");
            return null;
        }
    }

    public void AddItem(Item item) {
        if (!allItems.ContainsKey(item.item_ID)) {
            allItems[item.item_ID] = item;
            //SaveAllItems(); //实现保存所有物品到 JSON 文件的逻辑
        }
    }
}
