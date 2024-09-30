using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloseButton : MonoBehaviour
{
    private  Button closeButton;

    private void Start() {
        closeButton = GetComponent<Button>();
        closeButton.onClick.AddListener(CloseCurrentCanvas);
    }


    private void CloseCurrentCanvas() {
        CanvasManager.Instance.CloseCanvas(this.GetComponentInParent<Canvas>().name);
    }
}
