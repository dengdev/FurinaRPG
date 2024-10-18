using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T> {
    private static T instance;

    public static T Instance {
        get { return instance; }
    }

    protected virtual void Awake() {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = this as T; //DontDestroyOnLoad(gameObject); // 可以重写，添加其以保持单例对象在场景切换时不被销毁
    }

    public static bool IsInitialized {
        get { return instance != null; }
    }

    protected virtual void OnDestroy() {
        if (instance == this) {
            instance = null;
        }
    }
}
