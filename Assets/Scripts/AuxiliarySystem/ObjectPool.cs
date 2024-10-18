using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool{
    private readonly Queue<GameObject> pool;    
    private readonly GameObject prefab;         
    private readonly Transform parent;          
    private readonly int maxSize;               
    private int currentSize;                    

    public ObjectPool(GameObject prefab, int initialSize, int maxSize, Transform parent = null) {
        this.prefab = prefab;
        this.maxSize = maxSize;
        this.parent = parent;
        pool = new Queue<GameObject>();

        for (int i = 0; i < initialSize; i++) {
            AddToPool(CreateNewObject());
        }
    }

    public GameObject GetFromPool() {
        if (pool.Count > 0) {
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        } else if (currentSize < maxSize) {
            GameObject newObj = CreateNewObject();
            newObj.SetActive(true);
            return newObj;
        } else {
            Debug.LogWarning("Object pool is at maximum capacity!");
            GameObject newObj = CreateNewObject();
            newObj.SetActive(true);
            return newObj;
        }
    }

    public void ReturnToPool(GameObject obj) {
        if (obj.TryGetComponent<Rigidbody>(out Rigidbody rb)) {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        if (currentSize > maxSize) {
            GameObject.Destroy(obj);
        } else {
            obj.SetActive(false);
            AddToPool(obj);
        }
    }

    private GameObject CreateNewObject() {
        GameObject newObject = GameObject.Instantiate(prefab, parent);
        newObject.SetActive(false);
        currentSize++;
        return newObject;
    }

    private void AddToPool(GameObject obj) {
        pool.Enqueue(obj); 
    }
}
