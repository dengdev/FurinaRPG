using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ��ҵ�Ѫ���;���ֵUI
/// </summary>
public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private Text levelText; // �ȼ��ı�
    [SerializeField] private Text healthText; // Ѫ���ı�
    [SerializeField] private Image healthSlider; // Ѫ����
    [SerializeField] private Image expSlider; // ������

    private void Awake()
    {
        // ��ʼ�����
        healthSlider=transform.GetChild(0).GetChild(0).GetComponent<Image>();
        expSlider = transform.GetChild(1).GetChild(0).GetComponent<Image>();
        levelText = transform.GetChild(2).GetComponent<Text>();
        healthText = transform.GetChild(3).GetComponent<Text>();
    }

    private void Start()
    {
        // ��ʼ����ʾ
        //UpdateLevel();
        //UpdateHealth();
        //UpdateExp();
    }

    private void Update()
    {
        // ������Щֵ��Ƶ���仯������Ҫÿ֡����
        UpdateLevel();
        UpdateHealth();
        UpdateExp();
    }

    private void UpdateLevel()
    {
        // ���µȼ���ʾ
        levelText.text = "Lv:" + GameManager.Instance.playerStats.characterData.currentLevel.ToString("00");
    }

    private void UpdateHealth()
    {
        // ����Ѫ�������ı���ʾ
        float sliderPercent = (float)GameManager.Instance.playerStats.CurrentHealth / GameManager.Instance.playerStats.MaxHealth;
        healthSlider.fillAmount = sliderPercent;
        healthText.text = $"{GameManager.Instance.playerStats.CurrentHealth} / {GameManager.Instance.playerStats.MaxHealth}";
    }

    private void UpdateExp()
    {
        // ���¾�������ʾ
        float sliderPercent = (float)GameManager.Instance.playerStats.characterData.currentExp  / GameManager.Instance.playerStats.characterData.baseExp ;
        expSlider.fillAmount = sliderPercent;
    }
}
