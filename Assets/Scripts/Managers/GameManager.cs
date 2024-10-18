using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>, ISaveable {
    public CharacterStats playerStats;
    public PlayerData playerData;

    private List<IGameOverObserver> endGameObservers = new List<IGameOverObserver>();

    public ObjectPool rockPool;
    public ObjectPool enemyHPPool;
    public ObjectPool damageTextPool;

    protected override void Awake() {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start() {
        ISaveable saveable = this;
        saveable.AutoRegisteSaveable();
    }

    public void RegisterPlayer(CharacterStats player) {
        LoadOrInitializePlayerData();

        if (playerStats != null) { // ����Ƿ��Ѿ�ע�������
            Debug.LogWarning("�����ע�ᣬ�޷��ظ�ע�ᡣ");
            return;
        }

        playerStats = player;
        playerStats.characterData = playerData;
    }

    private void LoadOrInitializePlayerData() {
        if (!SaveManager.IsInitialized) {
            Debug.LogError("SaveManager ��δ��ʼ����������Ϸ����Ĵ���˳��");
            return;
        }
        SaveManager.Instance.Load();

        if (playerData == null) {
            Debug.Log("δ�ҵ������������ݣ����г�ʼ��");
            InitializePlayerData(); // ���г�ʼ��
        } else {
            Debug.Log("�ɹ��������������");
        }
    }

    private void InitializePlayerData() {
        playerData = new PlayerData(
            currentLevel: 1,
            maxLevel: 20,
            baseExp: 80,
            currentExp: 0,
            levelBuff: 0.1f,
            maxHealth: 100,
            currentHealth: 100,
            baseDefence: 2,
            currentDefence: 2,
            new List<Item>()
        );
        Debug.Log("������ݳ�ʼ��������");
        SaveManager.Instance.Save();
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
        if (playerData == null) {
            Debug.LogWarning ($"���ɱ�������ʱ��{this.name}��playerDataΪ��");
            return null; 
        }
        return new GameSaveData {playerData = this.playerData,};
    }

    public void RestoreGameData(GameSaveData gameSaveData) {
        playerData = gameSaveData.playerData;
    }

    private void OnApplicationQuit() {
        SaveManager.Instance.Save();
    }
}
