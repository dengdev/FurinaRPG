using System;
using System.Collections.Generic;
using UnityEngine;

public static class EventManager {
    private static Dictionary<string, Delegate> eventTable = new Dictionary<string, Delegate>();

    // 订阅事件，支持泛型
    public static void Subscribe<T>(string eventName, Action<T> listener) {
        if (string.IsNullOrEmpty(eventName) || listener == null) {
            throw new ArgumentNullException("事件名称或监听器不能为空");
        }

        if (!eventTable.ContainsKey(eventName)) {
            eventTable[eventName] = listener;
        } else {
            if (eventTable[eventName] is Action<T> existingDelegate) {
                //Delegate.Combine 方法允许多个回调函数订阅同一个事件，形成多播委托，即同一个事件可以触发多个回调函数
                eventTable[eventName] = Delegate.Combine(existingDelegate, listener);
            } else {
                throw new InvalidOperationException($"事件 {eventName} 的签名不匹配.");
            }
        }
    }

    public static void Subscribe(string eventName, Action listener) {
        if (string.IsNullOrEmpty(eventName) || listener == null) {
            throw new ArgumentNullException("事件名称或监听器不能为空");
        }

        if (!eventTable.ContainsKey(eventName)) {
            eventTable[eventName] = listener;
        } else {
            if (eventTable[eventName] is Action existingDelegate) {
                eventTable[eventName] = Delegate.Combine(existingDelegate, listener);
            } else {
                throw new InvalidOperationException($"事件 {eventName} 的签名不匹配.");
            }
        }
    }



    // 取消订阅事件
    public static void Unsubscribe<T>(string eventName, Action<T> listener) {
        if (string.IsNullOrEmpty(eventName) || listener == null) {
            throw new ArgumentNullException("事件名称或监听器不能为空");
        }

        if (eventTable.ContainsKey(eventName)) {
            //当前已经订阅了这个事件的所有回调函数的集合
            var currentDelegate = eventTable[eventName];

            if (currentDelegate is Action<T> existingDelegate) {
                //Delegate.Remove 会从委托链中移除指定的回调函数，其他的回调函数不受影响
                var newDelegate = Delegate.Remove(existingDelegate, listener);
                if (newDelegate == null) {
                    eventTable.Remove(eventName);
                } else {
                    eventTable[eventName] = newDelegate;
                }
            } else {
                throw new InvalidOperationException($"事件 {eventName} 的签名不匹配.");
            }
        }
    }
    public static void Unsubscribe(string eventName, Action listener) {
        if (string.IsNullOrEmpty(eventName) || listener == null) {
            throw new ArgumentNullException("事件名称或监听器不能为空");
        }

        if (eventTable.ContainsKey(eventName)) {
            var currentDelegate = eventTable[eventName];

            if (currentDelegate is Action existingDelegate) {
                var newDelegate = Delegate.Remove(existingDelegate, listener);
                if (newDelegate == null) {
                    eventTable.Remove(eventName);
                } else {
                    eventTable[eventName] = newDelegate;
                }
            } else {
                throw new InvalidOperationException($"事件 {eventName} 的签名不匹配.");
            }
        }
    }


    // 发布事件，安全调用
    public static void Publish<T>(string eventName, T eventData = default) {
        if (eventTable.ContainsKey(eventName)) {
            if (eventTable[eventName] is Action<T> callback) {
                try {
                    callback?.Invoke(eventData);
                } catch (Exception ex) {
                    Debug.LogError($"事件 {eventName} 执行时发生错误: {ex}");
                }
            } else {
                throw new InvalidOperationException($"事件 {eventName} 的签名不匹配.");
            }
        } else {
            Debug.LogWarning($"事件 {eventName} 没有订阅者.");
        }
    }

    public static void Publish(string eventName) {
        if (eventTable.ContainsKey(eventName)) {
            if (eventTable[eventName] is Action callback) {
                try {
                    callback?.Invoke();
                } catch (Exception ex) {
                    Debug.LogError($"事件 {eventName} 执行时发生错误: {ex}");
                }
            } else {
                throw new InvalidOperationException($"事件 {eventName} 的签名不匹配.");
            }
        } else {
            Debug.LogWarning($"事件 {eventName} 没有订阅者.");
        }
    }
}

/// <summary>
/// 可以封装事件数据
/// </summary>
public class EventData {
    public string eventName;
    public object data;

    public EventData(string name, object eventData) {
        eventName = name;
        data = eventData;
    }
}