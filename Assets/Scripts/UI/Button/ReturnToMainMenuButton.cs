using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReturnToMainMenuButton : MonoBehaviour
{
    void Start() {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(ReturnToMainMenu);

    }

    void ReturnToMainMenu() {
        SceneController.Instance.ReturnMenuScene();
    }
}
