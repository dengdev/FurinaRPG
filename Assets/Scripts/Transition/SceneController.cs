using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : Singleton<SceneController> {

    public SceneFader sceneFaderPrefab;
    [SerializeField] private string playerPrefabPath = "Prefabs/Player";
    private Transform playerTransform;
    //private string sceneFaderPrefabPath = "UI/FadeCanvas";

    private float fadeOutTime = 1.2f;
    private float fadeInTime = 1.0f;


    protected override void Awake() {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);
    }


    public void TransitionToDestination(TransitionPoint transitionPoint) {
        switch (transitionPoint.transitionType) {
            case TransitionPoint.TransitionType.SameScene:
                StartCoroutine(Transition(SceneManager.GetActiveScene().name, transitionPoint.destinationTag));
                break;
            case TransitionPoint.TransitionType.DifferentScene:
                StartCoroutine(Transition(transitionPoint.destinationSceneName, transitionPoint.destinationTag));
                break;
        }
    }

    IEnumerator Transition(string targetSceneName, TransitionDestination.DestinationTag destinationTag) {
        SaveManager.Instance.SavePlayerData();
        destinationCache?.Clear();

        if (SceneManager.GetActiveScene().name != targetSceneName) {
            yield return SceneManager.LoadSceneAsync(targetSceneName);

            UpdateDestinationCache();

            TransitionDestination destination = UseTagGetDestination(destinationTag);
            if (destination == null) {
                Debug.LogError("未找到匹配的传送目的地");
                yield break;
            }

            if (GameObject.FindGameObjectWithTag("Player") == null) {
                if (ResourceManager.IsInitialized) {
                    GameObject player = ResourceManager.Instance.InstantiateResource(playerPrefabPath, destination.transform.position, destination.transform.rotation);
                    yield return player;
                }
            }

            SaveManager.Instance.LoadPlayerData();
        } else {
            TransitionDestination destination = UseTagGetDestination(destinationTag);
            if (destination == null) { Debug.LogError("未找到匹配的传送目的地"); yield break; }

            playerTransform = GameManager.Instance.playerStats.transform;
            CharacterController controller = playerTransform.GetComponent<CharacterController>();
            if (controller != null) {
                if (controller.enabled) {
                    controller.enabled = false;
                    playerTransform.transform.SetPositionAndRotation(destination.transform.position, destination.transform.rotation);
                    controller.enabled = true;
                }
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

        Debug.LogWarning("未找到对应标签的目的地: " + destinationTag);
        return null;
    }

    public void TransitionToFirstScene() {
        StartCoroutine(LoadScene("SampleScene"));
    }

    public void TransitionToMainMenuScene() {
        StartCoroutine(LoadMainMenu());
    }

    public void TransitioToLoadScene() {
        StartCoroutine(LoadScene(SaveManager.Instance.CurrentSceneName));
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

            SaveManager.Instance.SavePlayerData();
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

}
