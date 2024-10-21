using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class EnemyHealthBarUI : MonoBehaviour {
    [Header("����Ѫ������")]
    public bool isAlwaysShow;
    public float showTime = 2.0f;
    public float changeTime = 0.9f;

    private float showTimer;
    private Transform hpBarPointAtEnemyHead;
    private GameObject EnemyHPbar;
    private Image changeHP;
    private Image currentHP;
    private CharacterStats enemStats_HpBar;
    private ObjectPool enemyHpPool;

    private void Awake() {
        InitializeHealthBar();
    }

    private void OnEnable() {
        EventManager.Subscribe("EnemyDeath", HideEnemyHPUI);
        EventManager.Subscribe<(int,int)>("ChangeEnemyHp", UpdateHealthBarUI);

    }

    private void Start() {
        if (enemStats_HpBar != null) {
            currentHP.fillAmount = enemStats_HpBar.CurrentHealth / enemStats_HpBar.MaxHealth;
            changeHP.fillAmount = currentHP.fillAmount;
        }
    }

    private void OnDisable() {
        EventManager.Unsubscribe("EnemyDeath", HideEnemyHPUI);
        EventManager.Unsubscribe<(int, int)>("ChangeEnemyHp", UpdateHealthBarUI);
    }

    private void LateUpdate() {
        if (EnemyHPbar != null) {
            EnemyHPbar.transform.position = hpBarPointAtEnemyHead.position;
            EnemyHPbar.transform.forward = Camera.main.transform.forward;
            AutoHideHpBar();
        }
    }

    private void InitializeHealthBar() {
        enemStats_HpBar = GetComponent<CharacterStats>();
        hpBarPointAtEnemyHead = transform.Find("HealthBar Point");

        if (hpBarPointAtEnemyHead == null) {
            Debug.LogError("δ�ҵ�����ͷ���� 'HealthBar Point' �ڵ�: " + gameObject.name);
            return;
        }
        GameObject healthUIPrefab = ResourceManager.Instance.LoadResource<GameObject>("Prefabs/UI/EnemyHPBar");

        // ͨ��������Ⱦģʽ�ҵ�Ѫ�����صĻ�����
        foreach (Canvas canvas in FindObjectsOfType<Canvas>()) {
            if (canvas.renderMode == RenderMode.WorldSpace) {
                enemyHpPool = GameManager.Instance.enemyHPPool?? new ObjectPool(healthUIPrefab, 5, 20, canvas.transform);
                GameManager.Instance.enemyHPPool=enemyHpPool;
                EnemyHPbar = enemyHpPool.GetFromPool();
                changeHP = EnemyHPbar.transform.GetChild(0).GetComponent<Image>();
                currentHP = EnemyHPbar.transform.GetChild(1).GetComponent<Image>();
                EnemyHPbar.SetActive(isAlwaysShow);
            }
        }
    }

    private void AutoHideHpBar() {

        if (showTimer <= 0 && !isAlwaysShow) {
            EnemyHPbar.SetActive(false);
        } else {
            showTimer -= Time.deltaTime;
        }
    }

    private void UpdateHealthBarUI((int currentHealth, int maxHealth) healthData) {
        float healthPercent = (float)healthData.currentHealth / healthData.maxHealth;

        if (healthData.currentHealth > 0) {
            EnemyHPbar.SetActive(true);
            showTimer = showTime;
        }
        currentHP.fillAmount = healthPercent;
        changeHP.DOFillAmount(healthPercent, changeTime).SetEase(Ease.OutQuad);
    }

    private void HideEnemyHPUI() {
        enemyHpPool.ReturnToPool(EnemyHPbar);
    }

    // ʹ�� DOTween ���Э�̽���Ѫ����䶯��������ʹ��Э���������Ŀ���
    private IEnumerator ChangeHealthBarFillAmount(Image slider, float targetFillAmount, float duration) {
        float startFillAmount = slider.fillAmount;
        float elapsedTime = 0f;

        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            slider.fillAmount = Mathf.Lerp(startFillAmount, targetFillAmount, elapsedTime / duration);
            yield return null;
        }
        slider.fillAmount = targetFillAmount;
    }
}
