using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private Text levelText; // 等级文本
    [SerializeField] private Text healthText; // 血量文本
    [SerializeField] private Image healthSlider; // 血量条
    [SerializeField] private Image expSlider; // 经验条

    private void Awake()
    {
        // 初始化组件
        healthSlider=transform.GetChild(0).GetChild(0).GetComponent<Image>();
        expSlider = transform.GetChild(1).GetChild(0).GetComponent<Image>();
        levelText = transform.GetChild(2).GetComponent<Text>();
        healthText = transform.GetChild(3).GetComponent<Text>();
    }

    private void Start()
    {
        // 初始化显示
        //UpdateLevel();
        //UpdateHealth();
        //UpdateExp();
    }

    private void Update()
    {
        // 假设这些值会频繁变化，才需要每帧更新
        UpdateLevel();
        UpdateHealth();
        UpdateExp();
    }

    private void UpdateLevel()
    {
        // 更新等级显示
        levelText.text = "Lv:" + GameManager.Instance.playerStats.characterData.currentLevel.ToString("00");
    }

    private void UpdateHealth()
    {
        // 更新血量条和文本显示
        float sliderPercent = (float)GameManager.Instance.playerStats.CurrentHealth / GameManager.Instance.playerStats.MaxHealth;
        healthSlider.fillAmount = sliderPercent;
        healthText.text = $"{GameManager.Instance.playerStats.CurrentHealth} / {GameManager.Instance.playerStats.MaxHealth}";
    }

    private void UpdateExp()
    {
        // 更新经验条显示
        float sliderPercent = (float)GameManager.Instance.playerStats.characterData.currentExp  / GameManager.Instance.playerStats.characterData.baseExp ;
        expSlider.fillAmount = sliderPercent;
    }
}
