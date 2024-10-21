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
            Debug.LogWarning("�������Ϊ��");
            return;
        }
        _PlayerData = GameManager.Instance.playerData;

        if (_PlayerData != null) {
            UpdateHpBarUI((_PlayerData.currentHealth, _PlayerData.maxHealth));
            UpdateExpUI(0); // ��ʼ��������
            UpdateLevelUI(_PlayerData.level); // ��ʼ���ȼ���ʾ
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
        StartCoroutine(ChangeFillAmount(_HPBackGroundSlider, hPPercent, 0.5f, false)); // 0.5���ڱ仯
    }

    private void UpdateExpUI(int exp) {
        Debug.Log($"���¾���UI{exp}");
        float currentExp = _PlayerData.currentExp;
        float baseExp = _PlayerData.baseExp;
        float eXPPercent = currentExp / baseExp;
        StartCoroutine(ChangeFillAmount(_EXPSlider, eXPPercent, 0.5f, true)); // 0.5���ڱ仯
    }

    private IEnumerator ChangeFillAmount(Image slider, float target, float duration, bool isExperienceBar) {
        float start = slider.fillAmount;
        float time = 0;

        // �����������ʱ�����Ƚ�������
        if (isExperienceBar && start > target) {
            while (time < duration) {
                time += Time.deltaTime;
                slider.fillAmount = Mathf.Lerp(start, 1, time / duration);
                yield return null;
            }
            slider.fillAmount = 1;
            time = 0;
            // ����ﵽ���޺���µȼ���Ѫ��
            UpdateHpBarUI((_PlayerData.currentHealth, _PlayerData.maxHealth));

            while (time < duration) {
                time += Time.deltaTime;
                slider.fillAmount = Mathf.Lerp(0, target, time / duration);
                yield return null;
            }
        } else {
            // ��������¾�������Ѫ����ƽ�����
            while (time < duration) {
                time += Time.deltaTime;
                slider.fillAmount = Mathf.Lerp(start, target, time / duration);
                yield return null;
            }
        }
        slider.fillAmount = target; // ȷ������ֵ
    }

    private void UpdateLevelUI(int level) {
        _LevelText.text = "Lv:" + level.ToString("00");
    }
}
