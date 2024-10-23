using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class MessageManager : MonoBehaviour {
    public GameObject messagePanel;
    public GameObject messagePrefab;
    [SerializeField] private int maxMessages = 5;
    [SerializeField] private float messageActivationInterval = 0.5f;

    private Queue<string> messageQueue = new Queue<string>();
    private List<GameObject> activeMessages = new List<GameObject>();
    private Coroutine messageActivationCoroutine;

    private void OnEnable() {
        EventManager.Subscribe<int>("PlayerGainExp", HandlePlayerGainExp);
        EventManager.Subscribe<(Item, int)>("PlayerGainItem", HandlePlayerGainItem);
    }

    private void OnDisable() {
        EventManager.Unsubscribe<int>("PlayerGainExp", HandlePlayerGainExp);
        EventManager.Unsubscribe<(Item, int)>("PlayerGainItem", HandlePlayerGainItem);
    }

    private void HandlePlayerGainExp(int exp) {
        ShowMessage($"获得经验: {exp} 点");
    }

    private void HandlePlayerGainItem((Item, int) itemInfo) {
        ShowMessage($"获得物品: {itemInfo.Item1.itemName} x{itemInfo.Item2}");
    }

    public void ShowMessage(string message) {
        messageQueue.Enqueue(message);
        if (activeMessages.Count < maxMessages && messageActivationCoroutine == null) {
            messageActivationCoroutine = StartCoroutine(ActivateMessages());
        }
    }

    private IEnumerator ActivateMessages() {
        while (messageQueue.Count > 0) {
            if (activeMessages.Count < maxMessages) {
                string nextMessage = messageQueue.Dequeue();
                DisplayMessage(nextMessage);
            }
            yield return new WaitForSeconds(messageActivationInterval);
        }

        messageActivationCoroutine = null;
    }

    private void DisplayMessage(string message) {
        if (!messagePanel.activeSelf) {
            messagePanel.SetActive(true);
        }

        GameObject newMessage = Instantiate(messagePrefab, messagePanel.transform);
        TMP_Text messageText = newMessage.GetComponent<TMP_Text>();
        messageText.text = message;

        float initialY = -50f * activeMessages.Count + 400f; // 400 是开始的 Y 坐标
        activeMessages.Add(newMessage);

        MessageAnimator animator = newMessage.GetComponent<MessageAnimator>();
        if (animator != null) {
            // 订阅事件以清除消息
            animator.OnMessageRemoved += () => {
                RemoveMessage(newMessage, animator);
                RearrangeMessages();
            };
            animator.PlayMessageAnimation(initialY); // 将初始 Y 位置传递给动画
        }
    }

    private void RemoveMessage(GameObject messageObj, MessageAnimator animator) {
        if (animator != null) {
            animator.StopAnimation(); // 停止动画
            animator.OnMessageRemoved -= () => RemoveMessage(messageObj, animator);
        }
        activeMessages.Remove(messageObj);
        Destroy(messageObj);

        if (activeMessages.Count == 0) {
            messagePanel.SetActive(false);
        } else {
            RearrangeMessages(); // 更新位置
        }
    }

    private void RearrangeMessages() {
        for (int i = 0; i < activeMessages.Count; i++) {
            if (activeMessages[i] != null) {
                RectTransform rectTransform = activeMessages[i].GetComponent<RectTransform>();
                if (rectTransform != null) {
                    // 更新位置时，确保 y 轴上的位置根据其在列表中的索引正确更新
                    float targetY = i * -50f + 400f;
                    if (rectTransform != null) {
                        rectTransform.DOLocalMoveY(targetY, 0.5f).SetEase(Ease.OutQuad); // 更新位置
                    }
                }
            }
        }
    }
}
