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
    public ReadOnlyDictionary<int, Item> allItems; // ʹ���ֵ�洢ȫ����Ʒ�б�
    public ReadOnlyDictionary<string, EnemyData> allEnemyData;

    private Dictionary<int, Item> itemDictionary; // ˽���ֵ�洢������Ʒ
    private Dictionary<string, EnemyData> enemyDictionary; // ˽���ֵ�洢���е���

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
        //Ҫȷ��Ψһ;
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
            Debug.Log("����ɹ�");
        } catch (Exception ex) {
            Debug.LogError($"����ʧ��: {ex.Message}");
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
            Debug.Log("���سɹ�");
        } catch (Exception ex) {
            Debug.LogError($"����ʧ��: {ex.Message}");
        }
    }

    private void Update() {

        if (Input.GetKeyDown(KeyCode.R)) {
            Debug.Log("����R�����������");
            Save();
        }

        if (Input.GetKeyDown(KeyCode.L)) {
            Debug.Log("����L��ȡ�������");
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
            Debug.LogError("δ���ҵ� JSON �ļ�: " + path);
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
            Debug.LogError("δ���ҵ� JSON �ļ�: " + path);
        }
    }

    public Item GetItem(int itemID) {
        allItems.TryGetValue(itemID, out Item item);
        if (item != null) {
            return item;
        } else {
            Debug.Log($"��{itemID}Ϊ��ЧID");
            return null;
        }
    }

    public EnemyData GetEnemy(string enemyName) {

        if (allEnemyData.TryGetValue(enemyName, out EnemyData enemy)) {
            EnemyData enemyCopy = new EnemyData(enemy.maxHealth, enemy.currentHealth, enemy.baseDefence, enemy.currentDefence, enemy.killPoint);
            return enemyCopy; // ��ȸ���
        } else {
            Debug.LogWarning($"�˵��� '{enemyName}' Ϊ��Ч����");
            return null;
        }
    }
}
