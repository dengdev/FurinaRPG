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
    private List<ISaveable> saveableList = new List<ISaveable>();

    private string jsonFolder;
    private Dictionary<string, GameSaveData> saveDataDictionary = new Dictionary<string, GameSaveData>();

    protected override void Awake() {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);
        jsonFolder = Path.Combine(Application.persistentDataPath, "/GameSaveData/");
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
}
