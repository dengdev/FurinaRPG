using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectPool {
    private Queue<GameObject> pool;
    private GameObject prefab;
    private Transform parent;
    private readonly int maxSize;
    private int totalSize;

    public ObjectPool(GameObject prefab, int initialSize, int maxSize, Transform parent = null) {
        this.prefab = prefab;
        this.maxSize = maxSize;
        this.parent = parent;
        pool = new Queue<GameObject>();

        for (int i = 0; i < initialSize; i++) {
            pool.Enqueue(CreateNewObject());
        }
        totalSize = initialSize;
    }

    public GameObject GetFromPool() {
        if (pool.Count > 0) {
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        } else if (totalSize < maxSize) {
            GameObject newObj = CreateNewObject();
            ResetObject(newObj);
            return newObj;
        } else {
            Debug.LogWarning("Object pool is at maximum capacity!");
            return null;
        }
    }

    public void ReturnToPool(GameObject obj) {
        if (obj == null || obj.Equals(null)) {
            Debug.LogWarning("Cannot return a destroyed object to the pool.");
            return;
        }

        obj.SetActive(false);
        pool.Enqueue(obj);
        Debug.Log(obj.name + " returned to pool.");
    }

    private GameObject CreateNewObject() {
        GameObject newObject = GameObject.Instantiate(prefab, parent != null ? parent : new GameObject().transform);
        newObject.SetActive(false);
        totalSize++;
        return newObject;
    }

    private void ResetObject(GameObject obj) {
        if (obj.TryGetComponent<Rigidbody>(out Rigidbody rb)) {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        obj.transform.position = Vector3.zero;
        obj.transform.rotation = Quaternion.identity;
    }

    private void ClearPool() {
        while (pool.Count > 0) {
            GameObject obj = pool.Dequeue();
            GameObject.Destroy(obj); // Ïú»Ù¶ÔÏó
            totalSize--;
        }
        Debug.Log("Cleared object pool.");
    }
}