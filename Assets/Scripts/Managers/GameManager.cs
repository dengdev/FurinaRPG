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

        if (playerStats != null) { // 检查是否已经注册了玩家
            Debug.LogWarning("玩家已注册，无法重复注册。");
            return;
        }

        playerStats = player;
        playerStats.characterData = playerData;
    }

    private void LoadOrInitializePlayerData() {
        if (!SaveManager.IsInitialized) {
            Debug.LogError("SaveManager 尚未初始化，请检查游戏对象的创建顺序。");
            return;
        }
        SaveManager.Instance.Load();

        if (playerData == null) {
            Debug.Log("未找到保存的玩家数据，进行初始化");
            InitializePlayerData(); // 进行初始化
        } else {
            Debug.Log("成功加载了玩家数据");
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
        Debug.Log("玩家数据初始化并保存");
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
        Debug.Log("没有找到入口");
        return null;
    }

    public GameSaveData GenerateSaveData() {
        if (playerData == null) {
            Debug.LogWarning ($"生成保存数据时，{this.name}的playerData为空");
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
