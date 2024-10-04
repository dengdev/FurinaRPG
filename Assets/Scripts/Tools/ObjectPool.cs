using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool{
    private readonly Queue<GameObject> pool;    // 用队列存储对象
    private readonly GameObject prefab;         // 预制体，用于实例化对象
    private readonly Transform parent;          // 对象池的父级物体，用于组织层级
    private readonly int maxSize;               // 最大容量
    private int currentSize;                    // 当前池子的对象数量

    // 构造函数，初始化对象池
    public ObjectPool(GameObject prefab, int initialSize, int maxSize, Transform parent = null) {
        this.prefab = prefab;
        this.maxSize = maxSize;
        this.parent = parent;
        pool = new Queue<GameObject>();

        // 预先实例化对象
        for (int i = 0; i < initialSize; i++) {
            AddToPool(CreateNewObject());
        }
    }

    // 从池中获取对象
    public GameObject GetFromPool() {
        if (pool.Count > 0) {
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        } else if (currentSize < maxSize) {
            GameObject newObj = CreateNewObject();
            newObj.SetActive(true); // 新创建的对象直接激活
            return newObj;
        } else {
            Debug.LogWarning("Object pool is at maximum capacity!");
            return null; // 如果池子已满，不再创建新对象
        }
    }

    // 将对象归还到池子中
    public void ReturnToPool(GameObject obj) {
        obj.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

        if (obj.TryGetComponent<Rigidbody>(out Rigidbody rb)) {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        if (currentSize > maxSize) {
            GameObject.Destroy(obj); // 超过最大容量则销毁对象
        } else {
            obj.SetActive(false);
            AddToPool(obj);
        }
    }

    // 创建一个新的对象实例
    private GameObject CreateNewObject() {
        GameObject newObject = GameObject.Instantiate(prefab, parent);
        newObject.SetActive(false); // 初始状态为非激活
        currentSize++;
        return newObject;
    }

    // 将对象添加到池子中
    private void AddToPool(GameObject obj) {
        pool.Enqueue(obj); // 将对象放入队列
    }
}
