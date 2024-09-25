using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour {
     private Text _LevelText;
     private Text _HPText;
     private Image _HPSlider; 
     private Image _EXPSlider; 

    private void Awake() {
        InitializeUIComponents();
    }

    private void InitializeUIComponents() {
        _HPSlider = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        _EXPSlider = transform.GetChild(1).GetChild(0).GetComponent<Image>();
        _LevelText = transform.GetChild(2).GetComponent<Text>();
        _HPText = transform.GetChild(3).GetComponent<Text>();
    }

    private void Update() {
        UpdateLevel();
        UpdateHP();
        UpdateEXP();
    }

    private void UpdateLevel() {
        _LevelText.text = "Lv:" + GameManager.Instance.playerStats.characterData.currentLevel.ToString("00");
    }

    private void UpdateHP() {
        float sliderPercent = (float)GameManager.Instance.playerStats.CurrentHealth / GameManager.Instance.playerStats.MaxHealth;
        _HPSlider.fillAmount = sliderPercent;
        _HPText.text = $"{GameManager.Instance.playerStats.CurrentHealth} / {GameManager.Instance.playerStats.MaxHealth}";
    }

    private void UpdateEXP() {
        float sliderPercent = (float)GameManager.Instance.playerStats.characterData.currentExp / GameManager.Instance.playerStats.characterData.baseExp;
        _EXPSlider.fillAmount = sliderPercent;
    }
}
