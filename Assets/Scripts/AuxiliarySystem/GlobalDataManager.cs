using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using System;
using System.IO;
using UnityEngine;

public class GlobalDataManager : Singleton<GlobalDataManager> {
    private Dictionary<int, Item> itemDictionary = new();
    private Dictionary<string, EnemyData> enemyDictionary = new();

    protected override void Awake() {
        base.Awake();
        DontDestroyOnLoad(this);
        LoadAllData();
    }

    private void LoadAllData() {
        LoadData<ItemList>("Resources/Data/items.json", itemList => {
            foreach (Item item in itemList.items) {
                itemDictionary[item.itemId] = item;
            }
        }, new ItemConverter());

        LoadData<EnemyDataList>("Resources/Data/enemies.json", enemyDataList => {
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
                : JsonConvert.DeserializeObject<T>(stringData);

            onDataLoaded(dataList);
        } else {
            Debug.LogError("未能找到 JSON 文件: " + resultPath);
        }
    }

    public Item GetItem(int itemID) {
        itemDictionary.TryGetValue(itemID, out Item item);
        if (item != null) {
            return item;
        } else {
            Debug.Log($"此{itemID}为无效ID");
            return null;
        }
    }

    public EnemyData GetEnemy(string enemyName) {
        if (enemyDictionary.TryGetValue(enemyName, out EnemyData enemy)) {
            EnemyData enemyCopy = new EnemyData(enemy.maxHealth, enemy.currentHealth, enemy.baseDefence, enemy.currentDefence, enemy.killPoint);
            return enemyCopy; // 深度复制
        } else {
            Debug.LogWarning($"此敌人 '{enemyName}' 为无效名称");
            return null;
        }
    }
}