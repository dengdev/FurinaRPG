using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuitGameButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(QuitGame);
        
    }

    // Update is called once per frame
    void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }
}
