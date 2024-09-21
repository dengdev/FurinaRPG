using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : Singleton<SaveManager> {

    private string currentSceneName = "sceneLevel";
    public string CurrentSceneName { get { return PlayerPrefs.GetString(currentSceneName); } }
    protected override void Awake() {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            SceneController.Instance.TransitionToMainMenuScene();
        }

        if (Input.GetKeyDown(KeyCode.R)) { SavePlayerData(); }
        if (Input.GetKeyDown(KeyCode.L)) { LoadPlayerData(); }
    }

    public void SavePlayerData() {
        Save(GameManager.Instance.playerStats.characterData, GameManager.Instance.playerStats.characterData.name);
    }

    public void LoadPlayerData() {
        Load(GameManager.Instance.playerStats.characterData, GameManager.Instance.playerStats.characterData.name);
    }

    public void Save(object data, string key) {
        var jsonData = JsonUtility.ToJson(data, true);
        PlayerPrefs.SetString(key, jsonData);
        PlayerPrefs.SetString(currentSceneName, SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();
    }

    public void Load(object data, string key) {
        if (PlayerPrefs.HasKey(key)) {
            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(key), data);
        }
    }
}
