using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : Singleton<SceneController>, ISaveable {

    public SceneFader sceneFaderPrefab;

    private Transform player;
    private float fadeOutTime = 1.2f;
    private float fadeInTime = 1.0f;
    private string currentSceneName;
    [SerializeField] private string playerPrefabPath = "Prefabs/Player";

    public string CurrentSceneName { get { return currentSceneName; } }

    protected override void Awake() {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start() {
        ISaveable saveable = this;
        saveable.RegisteSaveable();
    }

    public void TransitionToDestination(TransitionPoint transitionPoint) {
        switch (transitionPoint.transitionType) {
            case TransitionPoint.TransitionType.SameScene:
                StartCoroutine(Transition(SceneManager.GetActiveScene().name, transitionPoint.destinationTag));
                break;
            case TransitionPoint.TransitionType.DifferentScene:
                StartCoroutine(Transition(transitionPoint.destinationSceneName, transitionPoint.destinationTag));
                SaveManager.Instance.Save();
                break;
        }
    }

    IEnumerator Transition(string targetSceneName, TransitionDestination.DestinationTag destinationTag) {
        destinationCache?.Clear();
        if (SceneManager.GetActiveScene().name != targetSceneName) {
            // ͬ��������
            yield return SceneManager.LoadSceneAsync(targetSceneName);

            UpdateDestinationCache();

            TransitionDestination destination = UseTagGetDestination(destinationTag);
            if (destination == null) {
                Debug.LogError("δ�ҵ�ƥ��Ĵ���Ŀ�ĵ�");
                yield break;
            }

            if (GameObject.FindGameObjectWithTag("Player") == null) {
                if (ResourceManager.IsInitialized) {
                    GameObject player = ResourceManager.Instance.InstantiateResource(playerPrefabPath, destination.transform.position, destination.transform.rotation);
                    yield return player;
                }
            }
            SaveManager.Instance.Load();
        } else {
            // ��ͬ��������
            TransitionDestination destination = UseTagGetDestination(destinationTag);
            if (destination == null) { Debug.LogError("δ�ҵ�ƥ��Ĵ���Ŀ�ĵ�"); yield break; }

            player = GameManager.Instance.playerStats.transform;
            CharacterController controller = player.GetComponent<CharacterController>();
            if (controller != null) {
                controller.enabled = false;
                player.SetPositionAndRotation(destination.transform.position, destination.transform.rotation);
                controller.enabled = true;
            }
        }
    }

    private Dictionary<TransitionDestination.DestinationTag, TransitionDestination> destinationCache;

    private void UpdateDestinationCache() {
        if (destinationCache == null) {
            destinationCache = new Dictionary<TransitionDestination.DestinationTag, TransitionDestination>();
        }
        destinationCache.Clear();
        TransitionDestination[] entrances = FindObjectsOfType<TransitionDestination>();
        foreach (TransitionDestination entrance in entrances) {
            if (!destinationCache.ContainsKey(entrance.destinationtag)) {
                destinationCache.Add(entrance.destinationtag, entrance);
            }
        }
    }

    private TransitionDestination UseTagGetDestination(TransitionDestination.DestinationTag destinationTag) {
        if (destinationCache == null || destinationCache.Count == 0) {
            UpdateDestinationCache();
        }

        if (destinationCache.TryGetValue(destinationTag, out TransitionDestination destination)) {
            return destination;
        }

        Debug.LogWarning("δ�ҵ���Ӧ��ǩ��Ŀ�ĵ�: " + destinationTag);
        return null;
    }



    IEnumerator LoadScene(string sceneName) {
        //SceneFader sceneFader = ResourceManager.Instance.LoadResource<SceneFader>(sceneFaderPrefabPath);
        SceneFader fader = Instantiate(sceneFaderPrefab);
        if (!string.IsNullOrEmpty(sceneName)) {
            yield return StartCoroutine(fader.FadeOut(fadeOutTime));
            yield return SceneManager.LoadSceneAsync(sceneName);

            if (GameObject.FindGameObjectWithTag("Player") == null) {
                if (ResourceManager.IsInitialized) {
                    GameObject player = ResourceManager.Instance.InstantiateResource(playerPrefabPath, GameManager.Instance.GetEntrance().position, GameManager.Instance.GetEntrance().rotation);
                    yield return player;
                }
            }

            SaveManager.Instance.Save();
            yield return StartCoroutine(fader.FadeIn(fadeInTime));
            yield break;
        }
    }

    IEnumerator LoadMainMenu() {
        SceneFader fader = Instantiate(sceneFaderPrefab);
        yield return StartCoroutine(fader.FadeOut(fadeOutTime));
        yield return SceneManager.LoadSceneAsync("MainMenuScene");
        yield return StartCoroutine(fader.FadeIn(fadeInTime));
        yield break;
    }

    public GameSaveData GenerateSaveData() {

        GameSaveData saveData = new GameSaveData();
        currentSceneName = SceneManager.GetActiveScene().name;
        saveData.currentSceneName = SceneManager.GetActiveScene().name;
        return saveData;
    }

    public void RestoreGameData(GameSaveData saveData) {
        if (saveData == null) {
            Debug.LogError("��������Ϊ�գ��޷��ָ���");
            return;
        }
        currentSceneName = saveData.currentSceneName;
    }

    public void TransitionToFirstScene() {
        SaveManager.Instance.OnStartNewGame();
        StartCoroutine(LoadScene("SampleScene"));
    }

    public void TransitionToMainMenuScene() {
        StartCoroutine(LoadMainMenu());
    }

    public void TransitionToLoadScene() {
        StartCoroutine(LoadScene(CurrentSceneName));
    }
}
