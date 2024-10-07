using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System;
using System.Text;
using System.Collections.ObjectModel;

public class ItemList {
    public List<Item> items;
}

public class EnemyDataList {
    public List<EnemyData> enemies;
}

public class SaveManager : Singleton<SaveManager> {
    public ReadOnlyDictionary<int, Item> allItems; // 使用字典存储全局物品列表
    public ReadOnlyDictionary<string, EnemyData> allEnemyData;

    private Dictionary<int, Item> itemDictionary; // 私有字典存储所有物品
    private Dictionary<string, EnemyData> enemyDictionary; // 私有字典存储所有敌人

    private string jsonFolder;
    private List<ISaveable> saveableList = new List<ISaveable>();
    private Dictionary<string, GameSaveData> saveDataDictionary = new Dictionary<string, GameSaveData>();


    protected override void Awake() {
        base.Awake();
        jsonFolder = Application.persistentDataPath + "/GameSaveData/";
        DontDestroyOnLoad(this.gameObject);

        itemDictionary = new Dictionary<int, Item>();
        enemyDictionary = new Dictionary<string, EnemyData>();

        allItems = new ReadOnlyDictionary<int, Item>(itemDictionary);
        allEnemyData = new ReadOnlyDictionary<string, EnemyData>(enemyDictionary);
    }
    private void OnEnable() {

    }

    private void Start() {
        LoadAllItems();
        LoadAllEnemies();
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
            Debug.Log("保存成功");
        } catch (Exception ex) {
            Debug.LogError($"保存失败: {ex.Message}");
        }
    }

    public void Load() {
        try {
            string resultPath = jsonFolder + "data.sav";

            if (!File.Exists(resultPath)) { return; }

            string stringData = File.ReadAllText(resultPath);

            Dictionary<string, GameSaveData> jsonData = JsonConvert.DeserializeObject<Dictionary<string, GameSaveData>>(stringData);

            foreach (ISaveable saveable in saveableList) {
                saveable.RestoreGameData(jsonData[saveable.GetType().Name]);
            }
            Debug.Log("加载成功");
        } catch (Exception ex) {
            Debug.LogError($"加载失败: {ex.Message}");
        }
    }

    private void Update() {

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
            ItemList itemList = JsonConvert.DeserializeObject<ItemList>(stringData, new ItemConverter());

            foreach (Item item in itemList.items) {
                itemDictionary[item.item_ID] = item;
            }
        } else {
            Debug.LogError("未能找到 JSON 文件: " + path);
        }
    }

    private void LoadAllEnemies() {
        string path = Path.Combine(Application.dataPath, "Resources/Data/enemies.json");

        if (File.Exists(path)) {
            string stringData = File.ReadAllText(path, Encoding.UTF8);
            EnemyDataList enemyDataList = JsonConvert.DeserializeObject<EnemyDataList>(stringData);

            foreach (EnemyData enemy in enemyDataList.enemies) {
                enemyDictionary[enemy.characterName] = enemy;
            }
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

    public EnemyData GetEnemy(string enemyName) {

        if (allEnemyData.TryGetValue(enemyName, out EnemyData enemy)) {
            EnemyData enemyCopy = new EnemyData(enemy.maxHealth, enemy.currentHealth, enemy.baseDefence, enemy.currentDefence, enemy.killPoint);
            return enemyCopy; // 深度复制
        } else {
            Debug.LogWarning($"此敌人 '{enemyName}' 为无效名称");
            return null;
        }
    }
}
