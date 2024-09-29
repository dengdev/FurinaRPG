using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class MainMenu : MonoBehaviour {
    private Button startGameButton;
    private Button continueButton;
    private Button quitGameButton;
    private PlayableDirector playableDirector;

    private void Awake() {
        startGameButton = transform.GetChild(1).GetComponent<Button>();
        continueButton = transform.GetChild(2).GetComponent<Button>();
        quitGameButton = transform.GetChild(3).GetComponent<Button>();

        startGameButton.onClick.AddListener(PlayStartTimeline);
        continueButton.onClick.AddListener(ContinueGame);
        quitGameButton.onClick.AddListener(QuitGame);

        playableDirector = FindObjectOfType<PlayableDirector>();
        playableDirector.stopped += StartGame;
    }


    private void PlayStartTimeline() {
        playableDirector.Play();
    }

    private void StartGame(PlayableDirector ueslessObj) {
        PlayerPrefs.DeleteAll();
        SceneController.Instance.TransitionToFirstScene();
    }

    private void ContinueGame() {
        SceneController.Instance.TransitionToLoadScene();
    }


    private void QuitGame() {
        Application.Quit();
        Debug.Log("Quit Game");
    }
}
