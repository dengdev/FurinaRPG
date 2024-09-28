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
            Debug.Log("����Esc���ص����˵�");
            SceneController.Instance.TransitionToMainMenuScene();
        }

        if (Input.GetKeyDown(KeyCode.R)) { 
            
            Debug.Log("����R�����������");
            SavePlayerData(); }
        if (Input.GetKeyDown(KeyCode.L)) { 
            
            Debug.Log("����L��ȡ�������");
            LoadPlayerData(); }
    }

    public void SavePlayerData() {
        Save(GameManager.Instance.playerStats.characterData, GameManager.Instance.playerStats.characterData.name);
    }

    public void LoadPlayerData() {
        Debug.Log("���ڼ��ؽ�ɫ����");
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
