using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager> {
    public CharacterStats playerStats;

    private List<IGameOverObserver> endGameObservers = new List<IGameOverObserver>();

    protected override void Awake() {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);
    }

    // 第一次注册时数据未加载，第二注册才成功加载数据
    public void RegisterPlayer(CharacterStats player) {
        playerStats = player;
        if (playerStats.characterData != null) {
            GameObject.Find("PlayerHealth Canvas").transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    public void AddObserver(IGameOverObserver observer) {
        endGameObservers.Add(observer);
    }

    public void RemoveObserver(IGameOverObserver observer) {
        endGameObservers.Remove(observer);
    }

    public void NotifyObservers() {
        foreach (IGameOverObserver observer in endGameObservers) {
            observer.PlayerDeadNotify();
        }
    }

    public Transform GetEntrance() {
        foreach (TransitionDestination item in FindObjectsOfType<TransitionDestination>()) {
            if (item.destinationtag == TransitionDestination.DestinationTag.Enter) {
                return item.transform;
            }
        }
        Debug.Log("没有找到入口");
        return null;
    }
}
