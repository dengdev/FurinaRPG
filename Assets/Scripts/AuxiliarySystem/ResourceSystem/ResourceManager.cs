using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : Singleton<ResourceManager> {

    public T LoadResource<T>(string path) where T : Object {
        T resource = Resources.Load<T>(path);
        if (resource == null) {
            Debug.LogWarning($"没有找到此 {path} 资源路径!");
        }
        return resource;
    }

    public GameObject InstantiateResource(string path, Vector3 position, Quaternion rotation) {
        GameObject prefab = LoadResource<GameObject>(path);
        GameObject instantiatedObject= Instantiate(prefab, position, rotation);
        if (instantiatedObject == null) {
            Debug.LogError($"Failed to instantiate the object from prefab: {prefab.name}");
            return null;
        }
        instantiatedObject.name=prefab.name;
        return instantiatedObject;
    }

    protected override void Awake() {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);
    }
}
