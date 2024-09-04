using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    Text leveltext;
    Image healthSlider;
    Image expSlider;
    private void Awake()
    {
        leveltext=transform.GetChild(2).GetComponent<Text>();
        healthSlider=transform.GetChild(0).GetChild(0).GetComponent<Image>();
        expSlider = transform.GetChild(1).GetChild(0).GetComponent<Image>();

    }

    private void Update()
    {
        leveltext.text = "µÈ¼¶£º" + GameManager.Instance.playerStats.characterData.currentLevel.ToString("00");
        UpDateHealth();
        UpDateExp();
    }


    private void UpDateHealth()
    {
        float sliderPercent=(float)GameManager.Instance.playerStats.CurrentHealth/GameManager.Instance.playerStats.MaxHealth;
        healthSlider.fillAmount=sliderPercent;
    }

    private void UpDateExp()
    {
        float sliderPercent = (float)GameManager.Instance.playerStats.characterData.currentExp  / GameManager.Instance.playerStats.characterData.baseExp ;
        expSlider.fillAmount = sliderPercent;
    }
}
