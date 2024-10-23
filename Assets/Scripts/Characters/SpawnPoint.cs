using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour {
    [SerializeField] private string enemyName;
    [SerializeField] private int enemyCount = 5;
    [SerializeField] private float spawnRadius = 3f;
    [SerializeField] private float spawnInterval = 5.0f;
    private float acceleratedSpawnInterval;

    private float timer = 0f;
    private int spawnedEnemies = 0;
    private bool isPlayerInRange = false;

    private void Start() {
        acceleratedSpawnInterval = spawnInterval / 3f; 
    }

    private void Update() {
        timer += Time.deltaTime;
        float currentSpawnInterval = isPlayerInRange ? acceleratedSpawnInterval : spawnInterval;

        if (timer >= currentSpawnInterval && spawnedEnemies < enemyCount) {
            FactoryManager.Instance.randomSpawnEnemy(enemyName, 1, transform.position, spawnRadius);
            spawnedEnemies++;
            timer = 0f;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            isPlayerInRange = true;
            Debug.Log("玩家进入生成区域");
            spawnedEnemies = 0;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            isPlayerInRange = false;
            Debug.Log("玩家离开生成区域");
        }
    }
}