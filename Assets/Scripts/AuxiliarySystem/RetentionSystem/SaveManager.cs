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

    private Dictionary<int, Item> itemDictionary = new(); // 私有字典存储所有物品
    private Dictionary<string, EnemyData> enemyDictionary = new(); // 私有字典存储所有敌人

    private string jsonFolder;
    private List<ISaveable> saveableList = new List<ISaveable>();
    private Dictionary<string, GameSaveData> saveDataDictionary = new Dictionary<string, GameSaveData>();

    protected override void Awake() {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);
        jsonFolder = Path.Combine(Application.persistentDataPath, "/GameSaveData/");
        allItems = new ReadOnlyDictionary<int, Item>(itemDictionary);
        allEnemyData = new ReadOnlyDictionary<string, EnemyData>(enemyDictionary);
    }

    private void Start() {
        LoadAllData();
    }

    public void OnStartNewGame() {
        string resultPath = Path.Combine(jsonFolder, "data.sav");

        if (File.Exists(resultPath)) {
            File.Delete(resultPath);
        }
    }

    public void RegisterSaveable(ISaveable saveable) {
        if (!saveableList.Contains(saveable)) {
            saveableList.Add(saveable);
        }
    }

    public void Save() {
        try {
            saveDataDictionary.Clear();
            foreach (ISaveable saveable in saveableList) {
                //saveDataDictionary[saveable.GetType().Name] = saveable.GenerateSaveData();
                saveDataDictionary.Add(saveable.GetType().Name, saveable.GenerateSaveData());
            }

            string resultPath = jsonFolder + "data.sav";
            if (!Directory.Exists(resultPath)) {
                Directory.CreateDirectory(jsonFolder);
            }

            string jsonData = JsonConvert.SerializeObject(saveDataDictionary, Formatting.Indented);
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
            var jsonData = JsonConvert.DeserializeObject<Dictionary<string, GameSaveData>>(stringData);

            foreach (ISaveable saveable in saveableList) {
                if (jsonData.TryGetValue(saveable.GetType().Name, out GameSaveData gameSaveData)) {
                    saveable.RestoreGameData(gameSaveData);
                }
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

    private void LoadAllData() {

        LoadData<ItemList>("Resources/Data/items.json",
            itemList => {
                foreach (Item item in itemList.items) {
                    itemDictionary[item.itemId] = item;
                }
            },
            new ItemConverter());

        LoadData<EnemyDataList>("Resources/Data/enemies.json",
            enemyDataList => {
                foreach (EnemyData enemy in enemyDataList.enemies) {
                    enemyDictionary[enemy.characterName] = enemy;
                }
            });

    }

    private void LoadData<T>(string path, Action<T> onDataLoaded, JsonConverter converter = null) {
        string resultPath = Path.Combine(Application.dataPath, path);

        if (File.Exists(resultPath)) {
            string stringData = File.ReadAllText(resultPath, Encoding.UTF8);

            T dataList = converter != null
           ? JsonConvert.DeserializeObject<T>(stringData, converter)
           : JsonConvert.DeserializeObject<T>(stringData); // 默认反序列化

            onDataLoaded(dataList); // 使用委托处理加载后的数据

        } else {
            Debug.LogError("未能找到 JSON 文件: " + resultPath);
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
