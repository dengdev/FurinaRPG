using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryManager : Singleton<FactoryManager>
{
    public float spawnInterval = 1.0f;

    protected override void Awake() {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    public void SpawnEnemiesOverTime(string enemyName, int enemyCount, Vector3 spawnCenter, float spawnRadius) {
        StartCoroutine(SpawnEnemyCoroutine(enemyName, enemyCount, spawnCenter, spawnRadius));
    }

    private IEnumerator SpawnEnemyCoroutine(string enemyName, int enemyCount, Vector3 spawnCenter, float spawnRadius=3f) {
        for (int i = 0; i < enemyCount; i++) {
            // 生成随机位置，在中心点的半径范围内
            Vector3 randomPosition = spawnCenter + Random.insideUnitSphere * spawnRadius;
            randomPosition.y = spawnCenter.y;  // 保持Y轴不变，避免敌人生成在不同高度

            // 调用生成敌人的方法
            SpawnEnemy(enemyName, randomPosition, Quaternion.identity);

            // 等待设定的时间间隔再生成下一个敌人
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    public void SpawnEnemy(string enemyName, Vector3 position, Quaternion rotation) {
        GameObject obj = ResourceManager.Instance.InstantiateResource($"Prefabs/Enemy/{enemyName}", position, rotation);

        if (obj != null) {
            var characterStats = obj.GetComponent<CharacterStats>();
            if (characterStats != null) {
                characterStats.characterData = SaveManager.Instance.GetEnemy(obj.name);
            } else {
                Debug.LogError("CharacterStats 组件未在生成的对象上找到。");
            }
        } else {
            Debug.LogError($"生成 {enemyName} 预制体失败。");
        }
    }
}
