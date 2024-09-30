using UnityEngine;
using UnityEngine.UI;

public class SaveButton : MonoBehaviour {
    private Button savebutton;

    private void Start() {
        savebutton=GetComponent<Button>();
        savebutton.onClick.AddListener(SaveGame);
    }

    private void SaveGame() {
        SaveManager.Instance.Save();
        Debug.Log("”Œœ∑“—±£¥Ê£°");
    }
}