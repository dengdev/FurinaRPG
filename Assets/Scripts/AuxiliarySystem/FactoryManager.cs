using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryManager : Singleton<FactoryManager> {
    public float spawnInterval = 1.0f;

    protected override void Awake() {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    public void randomSpawnEnemy(string enemyName, int enemyCount, Vector3 spawnCenter, float spawnRadius = 3f) {

        for (int i = 0; i < enemyCount; i++) {
            Vector3 randomPosition = spawnCenter + Random.insideUnitSphere * spawnRadius;
            randomPosition.y = spawnCenter.y;  
            SpawnEnemy(enemyName, randomPosition, Quaternion.identity);
        }
    }

    private  void SpawnEnemy(string enemyName, Vector3 position, Quaternion rotation) {
        GameObject obj = ResourceManager.Instance.InstantiateResource($"Prefabs/Enemy/{enemyName}", position, rotation);

        if (obj != null) {
            var characterStats = obj.GetComponent<CharacterStats>();
            if (characterStats != null) {
                characterStats.characterData = GlobalDataManager.Instance.GetEnemy(obj.name);
            } else {
                Debug.LogError("CharacterStats 组件未在生成的对象上找到。");
            }
        } else {
            Debug.LogError($"生成 {enemyName} 预制体失败。");
        }
    }
}
