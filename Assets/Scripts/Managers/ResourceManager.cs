using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : Singleton<ResourceManager> {
    public T LoadResource<T>(string path) where T : Object {
        T resource = Resources.Load<T>(path);
        if (resource == null) {
            Debug.LogWarning($"Resource at path {path} could not be found!");
        }
        return resource;
    }

    public GameObject InstantiateResource(string path, Vector3 position, Quaternion rotation) {
        GameObject prefab = LoadResource<GameObject>(path);
        if (prefab == null) {
            Debug.LogError($"Failed to instantiate resource at path {path}");
            return null;
        }
        return Instantiate(prefab, position, rotation);
    }

    protected override void Awake() {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);
    }
}
