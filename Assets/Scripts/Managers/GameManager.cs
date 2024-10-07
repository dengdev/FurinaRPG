using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>, ISaveable {
    public CharacterStats playerStats;
    public PlayerData playerData;

    private List<IGameOverObserver> endGameObservers = new List<IGameOverObserver>();

    public ObjectPool rockPool;


    protected override void Awake() {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start() {
        ISaveable saveable = this;
        saveable.AutoRegisteSaveable();
    }

    private void Update() {

    }

    // ��һ��ע��ʱ����δ���أ��ڶ�ע��ųɹ���������
    public void RegisterPlayer(CharacterStats player) {
        playerStats = player;

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
        Debug.Log("û���ҵ����");
        return null;
    }

    public GameSaveData GenerateSaveData() {
        if (playerStats == null || playerStats.playerData == null) {
            Debug.LogWarning ("���ɱ�������ʱ�����ͳ����Ϣ���ɫ����δ��ʼ����");
            return null; // ���߷���һ��Ĭ��ֵ
        }
       
        GameSaveData saveData = new GameSaveData {
            playerData = this.playerData,
        };
        return saveData;
    }
    

    public void RestoreGameData(GameSaveData gameSaveData) {
        playerData = gameSaveData.playerData;
    }
}
