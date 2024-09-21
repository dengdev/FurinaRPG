using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
    private Button startGameButton;
    private Button continueButton;
    private Button quitGameButton;

    private void Awake() {
        startGameButton = transform.GetChild(1).GetComponent<Button>();
        continueButton = transform.GetChild(2).GetComponent<Button>();
        quitGameButton = transform.GetChild(3).GetComponent<Button>();

        startGameButton.onClick.AddListener(StartGame);
        continueButton.onClick.AddListener(ContinueGame);
        quitGameButton.onClick.AddListener(QuitGame);
    }


    private void StartGame() {
        PlayerPrefs.DeleteAll();
        SceneController.Instance.TransitionToFirstScene();
    }

    private void ContinueGame() {
        SceneController.Instance.TransitioToLoadScene();
    }


    private void QuitGame() {
        Application.Quit();
        Debug.Log("Quit Game");
    }
}
