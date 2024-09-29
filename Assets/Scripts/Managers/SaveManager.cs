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
    public Dictionary<int, Item> allItems; // ʹ���ֵ�洢ȫ����Ʒ�б�

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
        LoadAllItems(); // ����Ϸ��ʼʱ����������Ʒ��Ϣ


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

    /// <summary>
    /// �����ض�������
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
            Debug.LogError($"����ʧ��: {ex.Message}");
        }
    }

    /// <summary>
    /// ���ѱ���������м���
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
            Debug.LogError($"����ʧ��: {ex.Message}");
        }
    }





    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Debug.Log("����Esc���ص����˵�");
            SceneController.Instance.TransitionToMainMenuScene();
        }

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
            Debug.Log(stringData);
            ItemList itemList = JsonConvert.DeserializeObject<ItemList>(stringData, new ItemConverter());
            allItems = new Dictionary<int, Item>();

            foreach (Item item in itemList.items) {
                allItems[item.item_ID] = item;
            }
            Debug.Log("������Ʒ���ݼ��سɹ���");
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

    public void AddItem(Item item) {
        if (!allItems.ContainsKey(item.item_ID)) {
            allItems[item.item_ID] = item;
            //SaveAllItems(); //ʵ�ֱ���������Ʒ�� JSON �ļ����߼�
        }
    }
}
