using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : Singleton<SceneManager>, ISaveable {
    [SerializeField] private SceneFader sceneFaderPrefab;
    [SerializeField] private float fadeOutTime = 0.4f;
    [SerializeField] private float fadeInTime = 0.3f;
    [SerializeField] private string currentSceneName;
    private Dictionary<TransferAnchorTag, TransferAnchor> anchorCache = new();
    private bool isFirstGameStart = true;

    protected override void Awake() {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start() {
        ISaveable saveable = this;
        saveable.AutoRegisteSaveable();

        if (!isFirstGameStart) {
            SaveManager.Instance.Load();
            Debug.Log($"场景加载数据，当前场景为{currentSceneName}");
        } else {
            // 如果是第一次游玩，直接加载初始场景
            TransitionToFirstScene();
        }
        sceneFaderPrefab = ResourceManager.Instance.LoadResource<SceneFader>("Prefabs/Scene/FadeCanvas");

    }

    public void TransferToTargetAnchor(TransferAnchor anchor) {
        StartCoroutine(HandleSceneTransition(anchor));
    }

    IEnumerator HandleSceneTransition(TransferAnchor anchor) {
        // 检查传送门传送是否需要加载新场景
        if (!anchor.IsSameScene()) {
            yield return StartCoroutine(LoadSceneWithFade(anchor.crossScene));
            Debug.Log("传送锚点传送不同场景后自动保存");
            SaveManager.Instance.Save();
        } else {
            MovePlayerToAnchor(anchor);
        }
        yield return null;
    }

    private void MovePlayerToAnchor(TransferAnchor anchor) {
        Transform target = FindAnchorWithTag(anchor.destinationTag)?.transform;

        if (target == null) {
            Debug.Log("玩家要去的锚点为空");
            return;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) {
            ResourceManager.Instance.InstantiateResource("Prefabs/Player/Player", target.position, target.rotation);
        } else {
            // 改变玩家位置
            if (player.TryGetComponent<CharacterController>(out CharacterController controller)) {
                controller.enabled = false;
                GameManager.Instance.playerStats.transform.SetPositionAndRotation(target.position, target.rotation);
                controller.enabled = true;
            }
        }
    }

    private void UpdateAnchorCache() {
        anchorCache.Clear();
        foreach (TransferAnchor anchor in FindObjectsOfType<TransferAnchor>()) {
            if (!anchorCache.ContainsKey(anchor.anchorID)) {
                anchorCache.Add(anchor.anchorID, anchor);
            }
        }
    }

    private TransferAnchor FindAnchorWithTag(TransferAnchorTag anchorTag) {
        if (anchorCache.TryGetValue(anchorTag, out TransferAnchor anchor)) {
            return anchor;
        }
        Debug.LogWarning("未找到对应标签的目的地: " + anchorTag);
        return null;
    }

    IEnumerator LoadSceneWithFade(string sceneName, bool isMainMenu = false) {
        //清空状态
        DOTween.KillAll();
        GameManager.Instance.ClearPools();

        SceneFader fader = Instantiate(sceneFaderPrefab);
        yield return StartCoroutine(fader.FadeOut(fadeOutTime));
        yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        UpdateAnchorCache();
        currentSceneName = sceneName;
        if (!isMainMenu) {
            MovePlayerToAnchor(FindEnterAnchor());
        }
        yield return fader.FadeIn(fadeInTime);
    }

    public TransferAnchor FindEnterAnchor() {
        foreach (TransferAnchor anchor in FindObjectsOfType<TransferAnchor>()) {
            if (anchor.destinationTag == TransferAnchorTag.Enter) {
                return anchor;
            }
        }
        Debug.LogWarning("没有找到Enter入口");
        return null;
    }

    public GameSaveData GenerateSaveData() {
        Debug.Log($"保存的场景名字为 {currentSceneName}");
        return new GameSaveData() {
            currentSceneName = currentSceneName
        };
    }

    public void RestoreGameData(GameSaveData saveData) {
        if (saveData != null) {
            currentSceneName = saveData.currentSceneName;
            Debug.Log($"已恢复的场景名字为 {currentSceneName}");
        } else {
            Debug.LogError("保存数据为空，无法恢复。");
        }
    }

    public void TransitionToFirstScene() {
        SaveManager.Instance.OnStartNewGame();
        StartCoroutine(LoadSceneWithFade("SampleScene"));
    }

    public void ReturnMenuScene() {
        StartCoroutine(LoadSceneWithFade("MainMenuScene", true));
    }

    public void LoadSceneToContinueGame() {
        SaveManager.Instance.Load();
        if(currentSceneName == "MainMenuScene") {
            Debug.Log("暂时没有保存的数据");
            return;
        }
        if (!string.IsNullOrEmpty(currentSceneName)) {
            StartCoroutine(LoadSceneWithFade(currentSceneName));
            Debug.Log($"要去的场景{currentSceneName}为保存的场景");
        } else {
            Debug.LogWarning("当前场景名为空，无法加载保存的场景");
        }
    }
}
