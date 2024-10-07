using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour {
    private Text _LevelText;
    private Text _HPText;
    private Image _HPBackGroundSlider;
    private Image _HPSlider;
    private Image _EXPSlider;

    private bool isSubscribed = false;
    private  PlayerData _PlayerData;
    private void Awake() {
        InitializeUIComponents();
    }

    private void OnEnable() {
        if (!isSubscribed) {
            GameManager.Instance.playerStats.OnHealthChanged += PlayerStats_OnHealthChanged;
            GameManager.Instance.playerStats.OnGainExp += PlayerStats_OnGainExp;
            isSubscribed = true;
        }

        // 更新玩家数据
        _PlayerData = GameManager.Instance.playerData;
        PlayerStats_OnHealthChanged(_PlayerData.currentHealth, _PlayerData.maxHealth);
        PlayerStats_OnGainExp(0);
        UpdatePlayerLevel();
    }

    private void OnDisable() {
        if (GameManager.Instance != null && GameManager.Instance.playerStats != null && isSubscribed) {
            GameManager.Instance.playerStats.OnGainExp -= PlayerStats_OnGainExp;
            GameManager.Instance.playerStats.OnHealthChanged -= PlayerStats_OnHealthChanged;
            isSubscribed = false; // 更新订阅状态
        }
    }

    private void InitializeUIComponents() {
        _HPBackGroundSlider= transform.GetChild(1).GetComponent<Image>();
        _HPSlider = transform.GetChild(1).GetChild(0).GetComponent<Image>();
        _EXPSlider = transform.GetChild(2).GetChild(0).GetComponent<Image>();
        _LevelText = transform.GetChild(3).GetComponent<Text>();
        _HPText = transform.GetChild(4).GetComponent<Text>();
    }

    private void PlayerStats_OnHealthChanged(int curHP, int maxHP) {
        float hPPercent = (float)curHP / maxHP;
        _HPSlider.fillAmount = hPPercent;
        StartCoroutine(ChangeFillAmount(_HPBackGroundSlider, hPPercent, 0.5f, false)); // 0.5秒内变化
        _HPText.text = $"{curHP} / {maxHP}";
    }

    private void PlayerStats_OnGainExp(int exp) {
        float currentExp = _PlayerData.currentExp;
        float baseExp = _PlayerData.baseExp;
        float eXPPercent = currentExp / baseExp;
        StartCoroutine(ChangeFillAmount(_EXPSlider, eXPPercent, 0.5f, true)); // 0.5秒内变化
        
    }

    private IEnumerator ChangeFillAmount(Image slider, float target, float duration, bool isExp) {
        float start = slider.fillAmount;
        float time = 0;


        if (isExp && start > target) {
            while (time < duration) {
                time += Time.deltaTime;
                slider.fillAmount = Mathf.Lerp(start, 1, time / duration);
                yield return null;
            }
            slider.fillAmount = 1;
            time = 0;
            // 经验达到上限后更新等级和血量
            UpdatePlayerLevel();
            PlayerStats_OnHealthChanged(_PlayerData.currentHealth, GameManager.Instance.playerStats.playerData.maxHealth);

            while (time < duration) {
                time += Time.deltaTime;
                slider.fillAmount = Mathf.Lerp(0, target, time / duration);
                yield return null;
            }
        } else {

            while (time < duration) {
                time += Time.deltaTime;
                slider.fillAmount = Mathf.Lerp(start, target, time / duration);
                yield return null;
            }
        }
        slider.fillAmount = target; // 确保最终值
    }

    private void UpdatePlayerLevel() {
        _LevelText.text = "Lv:" + _PlayerData.currentLevel.ToString("00");
    }
}
