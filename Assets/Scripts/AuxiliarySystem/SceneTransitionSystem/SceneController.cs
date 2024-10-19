using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : Singleton<SceneController>, ISaveable {
    public SceneFader sceneFaderPrefab;

    private float fadeOutTime = 0.4f;
    private float fadeInTime = 0.3f;
    private string currentSceneName;
    [SerializeField] private string playerPrefabPath = "Prefabs/Player";
    private Dictionary<TransferAnchorTag, TransferAnchor> anchorCache = new();

    public string CurrentSceneName => currentSceneName;

    protected override void Awake() {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start() {
        ISaveable saveable = this;
        saveable.AutoRegisteSaveable();
        SaveManager.Instance.Load();
    }

    public void TransferToTargetAnchor(TransferAnchor anchor) {
        StartCoroutine(HandleSceneTransition(anchor));
    }

    IEnumerator HandleSceneTransition(TransferAnchor anchor) {
        // 检查传送门传送是否需要加载新场景
        if (!anchor.IsSameScene()) {
            yield return StartCoroutine(LoadSceneWithFade(anchor.crossScene));
            Debug.Log("不同场景传送后自动保存");
            SaveManager.Instance.Save();
        }

        AtAnchorAppear(FindAnchorWithTag(anchor.destinationTag).transform);
        yield return null;
    }

    private void AtAnchorAppear(Transform anchor) {
        if (GameObject.FindGameObjectWithTag("Player") == null) {
            // 重新生成玩家
            GameObject player = ResourceManager.Instance.InstantiateResource(playerPrefabPath, anchor.position, anchor.rotation);
        } else {
            // 改变玩家位置
            CharacterController controller = GameManager.Instance.playerStats.transform.GetComponent<CharacterController>();
            if (controller != null) {
                controller.enabled = false;
                GameManager.Instance.playerStats.transform.SetPositionAndRotation(anchor.position, anchor.rotation);
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
        SceneFader fader = Instantiate(sceneFaderPrefab);
        yield return StartCoroutine(fader.FadeOut(fadeOutTime));
        yield return SceneManager.LoadSceneAsync(sceneName);
        UpdateAnchorCache();
        currentSceneName = sceneName;
        AtAnchorAppear(FindAnchor());
        yield return fader.FadeIn(fadeInTime);
        if (isMainMenu) yield break;
    }

    public Transform FindAnchor() {
        foreach (TransferAnchor anchor in FindObjectsOfType<TransferAnchor>()) {
            if (anchor.destinationTag == TransferAnchorTag.Enter) {
                return anchor.transform;
            }
        }
        Debug.LogWarning("没有找到Enter入口");
        return null;
    }

    public GameSaveData GenerateSaveData() {
        return new GameSaveData() {
            currentSceneName = SceneManager.GetActiveScene().name
        };
    }

    public void RestoreGameData(GameSaveData saveData) {
        if (saveData != null) {
            currentSceneName = saveData.currentSceneName;
            Debug.Log($"恢复的场景名字为 {currentSceneName}");
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
        if (!string.IsNullOrEmpty(CurrentSceneName)) {
            StartCoroutine(LoadSceneWithFade(CurrentSceneName));
            Debug.LogWarning($"要去的场景{CurrentSceneName}为保存的场景");
        } else {
            Debug.LogWarning("当前场景名为空，无法加载保存的场景");
        }
    }
}
