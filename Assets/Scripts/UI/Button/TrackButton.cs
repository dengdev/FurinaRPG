using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrackButton : MonoBehaviour {
    private Button trackButton;
    private void Start() {
        trackButton = GetComponent<Button>();
        trackButton.onClick.AddListener(TrackQuest);
    }

    private void TrackQuest() {
        trackButton. GetComponentInChildren<TextMeshProUGUI>().text = "正在追踪";
    }
}
