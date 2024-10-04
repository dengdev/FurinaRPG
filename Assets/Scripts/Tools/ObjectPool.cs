using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool{
    private readonly Queue<GameObject> pool;    // �ö��д洢����
    private readonly GameObject prefab;         // Ԥ���壬����ʵ��������
    private readonly Transform parent;          // ����صĸ������壬������֯�㼶
    private readonly int maxSize;               // �������
    private int currentSize;                    // ��ǰ���ӵĶ�������

    // ���캯������ʼ�������
    public ObjectPool(GameObject prefab, int initialSize, int maxSize, Transform parent = null) {
        this.prefab = prefab;
        this.maxSize = maxSize;
        this.parent = parent;
        pool = new Queue<GameObject>();

        // Ԥ��ʵ��������
        for (int i = 0; i < initialSize; i++) {
            AddToPool(CreateNewObject());
        }
    }

    // �ӳ��л�ȡ����
    public GameObject GetFromPool() {
        if (pool.Count > 0) {
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        } else if (currentSize < maxSize) {
            GameObject newObj = CreateNewObject();
            newObj.SetActive(true); // �´����Ķ���ֱ�Ӽ���
            return newObj;
        } else {
            Debug.LogWarning("Object pool is at maximum capacity!");
            return null; // ����������������ٴ����¶���
        }
    }

    // ������黹��������
    public void ReturnToPool(GameObject obj) {
        obj.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

        if (obj.TryGetComponent<Rigidbody>(out Rigidbody rb)) {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        if (currentSize > maxSize) {
            GameObject.Destroy(obj); // ����������������ٶ���
        } else {
            obj.SetActive(false);
            AddToPool(obj);
        }
    }

    // ����һ���µĶ���ʵ��
    private GameObject CreateNewObject() {
        GameObject newObject = GameObject.Instantiate(prefab, parent);
        newObject.SetActive(false); // ��ʼ״̬Ϊ�Ǽ���
        currentSize++;
        return newObject;
    }

    // ��������ӵ�������
    private void AddToPool(GameObject obj) {
        pool.Enqueue(obj); // ������������
    }
}
