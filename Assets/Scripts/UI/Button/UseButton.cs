using UnityEngine;
using UnityEngine.UI;

public class UseButton : MonoBehaviour
{
    private Button button;
    private void Awake() {
        button = GetComponent<Button>();
        button.onClick.AddListener(UseItem);
    }

    private void UseItem() {
        FindObjectOfType<InventoryPanel>().UseCurrentSlot();

    }
}
