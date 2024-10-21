using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour {
    private Text _LevelText;
    private Text _HPText;
    private Image _HPBackGroundSlider;
    private Image _HPSlider;
    private Image _EXPSlider;

    private  PlayerData _PlayerData;

    private void Awake() {
        InitializeUIComponents();
    }

    private void OnEnable() {
        EventManager.Subscribe<int>("PlayerGainExp", UpdateExpUI);
        EventManager.Subscribe<int>("PlayerLevelUp", UpdateLevelUI);
        EventManager.Subscribe<(int, int)>("ChangePlayerHp", UpdateHpBarUI);

        if (GameManager.Instance == null || GameManager.Instance.playerData == null) {
            Debug.LogWarning("玩家数据为空");
            return;
        }
        _PlayerData = GameManager.Instance.playerData;

        if (_PlayerData != null) {
            UpdateHpBarUI((_PlayerData.currentHealth, _PlayerData.maxHealth));
            UpdateExpUI(0); // 初始化经验条
            UpdateLevelUI(_PlayerData.level); // 初始化等级显示
        }
    }

    private void OnDisable() {
        EventManager.Unsubscribe<int>("PlayerGainExp", UpdateExpUI);
        EventManager.Unsubscribe<int>("PlayerLevelUp", UpdateLevelUI);
        EventManager.Unsubscribe<(int, int)>("ChangePlayerHp", UpdateHpBarUI);
    }

    private void InitializeUIComponents() {
        _HPBackGroundSlider= transform.GetChild(1).GetComponent<Image>();
        _HPSlider = transform.GetChild(1).GetChild(0).GetComponent<Image>();
        _EXPSlider = transform.GetChild(2).GetChild(0).GetComponent<Image>();
        _LevelText = transform.GetChild(3).GetComponent<Text>();
        _HPText = transform.GetChild(4).GetComponent<Text>();
    }

    private void UpdateHpBarUI((int curHP, int maxHP) changeHp) {
        float hPPercent = (float)changeHp.curHP / changeHp.maxHP;
        _HPSlider.fillAmount = hPPercent;
        _HPText.text = $"{changeHp.curHP} / {changeHp.maxHP}";
        StartCoroutine(ChangeFillAmount(_HPBackGroundSlider, hPPercent, 0.5f, false)); // 0.5秒内变化
    }

    private void UpdateExpUI(int exp) {
        Debug.Log($"更新经验UI{exp}");
        float currentExp = _PlayerData.currentExp;
        float baseExp = _PlayerData.baseExp;
        float eXPPercent = currentExp / baseExp;
        StartCoroutine(ChangeFillAmount(_EXPSlider, eXPPercent, 0.5f, true)); // 0.5秒内变化
    }

    private IEnumerator ChangeFillAmount(Image slider, float target, float duration, bool isExperienceBar) {
        float start = slider.fillAmount;
        float time = 0;

        // 当经验条溢出时，首先将其填满
        if (isExperienceBar && start > target) {
            while (time < duration) {
                time += Time.deltaTime;
                slider.fillAmount = Mathf.Lerp(start, 1, time / duration);
                yield return null;
            }
            slider.fillAmount = 1;
            time = 0;
            // 经验达到上限后更新等级和血量
            UpdateHpBarUI((_PlayerData.currentHealth, _PlayerData.maxHealth));

            while (time < duration) {
                time += Time.deltaTime;
                slider.fillAmount = Mathf.Lerp(0, target, time / duration);
                yield return null;
            }
        } else {
            // 正常情况下经验条或血条的平滑填充
            while (time < duration) {
                time += Time.deltaTime;
                slider.fillAmount = Mathf.Lerp(start, target, time / duration);
                yield return null;
            }
        }
        slider.fillAmount = target; // 确保最终值
    }

    private void UpdateLevelUI(int level) {
        _LevelText.text = "Lv:" + level.ToString("00");
    }
}
