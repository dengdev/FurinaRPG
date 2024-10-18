using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class EnemyHealthBarUI : MonoBehaviour {
    [Header("敌人血条设置")]
    public bool isAlwaysShow;
    public float showTime = 2.0f;
    public float changeTime = 0.9f;

    private float showTimer;
    private Transform hpBarPointAtEnemyHead;
    private GameObject EnemyHPbar;
    private Image changeHP;
    private Image currentHP;
    private Transform mainCameraTransform;
    private CharacterStats enemStats_HPBar;

    private ObjectPool enemyHPPool;

    private void Awake() {
        InitializeHealthBar();
    }

    private void OnEnable() {
        mainCameraTransform = Camera.main.transform;

        if (enemStats_HPBar != null) {
            enemStats_HPBar.OnHealthChanged += UpdateHealthBarUI;
            enemStats_HPBar.OnDeath += HideHealthBarUI;
        }
    }

    private void Start() {
        if (enemStats_HPBar != null) {
            currentHP.fillAmount = enemStats_HPBar.CurrentHealth / enemStats_HPBar.MaxHealth;
            changeHP.fillAmount = currentHP.fillAmount;
        }
    }

    private void OnDisable() {
        if (enemStats_HPBar != null) {
            enemStats_HPBar.OnHealthChanged -= UpdateHealthBarUI;
            enemStats_HPBar.OnDeath -= HideHealthBarUI;
        }
    }

    private void LateUpdate() {
        if (EnemyHPbar != null) {
            UpdateUIPosition();
            AutoHideUI();
        }
    }

    private void InitializeHealthBar() {
        enemStats_HPBar = GetComponent<CharacterStats>();

        hpBarPointAtEnemyHead = transform.Find("HealthBar Point");
        if (hpBarPointAtEnemyHead == null) {
            Debug.LogError("未找到敌人头顶的 'HealthBar Point' 节点: " + gameObject.name);
            return;
        }

        GameObject healthUIPrefab = ResourceManager.Instance.LoadResource<GameObject>("Prefabs/UI/EnemyHPBar");

        // 通过查找渲染模式找到血条挂载的画布。
        foreach (Canvas canvas in FindObjectsOfType<Canvas>()) {
            if (canvas.renderMode == RenderMode.WorldSpace) {

                if (GameManager.Instance.enemyHPPool == null) {
                    GameManager.Instance.enemyHPPool = new ObjectPool(healthUIPrefab, 5, 20, canvas.transform);
                }

                enemyHPPool = GameManager.Instance.enemyHPPool;
                EnemyHPbar = enemyHPPool.GetFromPool();
                changeHP = EnemyHPbar.transform.GetChild(0).GetComponent<Image>();
                currentHP = EnemyHPbar.transform.GetChild(1).GetComponent<Image>();
                EnemyHPbar.SetActive(isAlwaysShow);
            }
        }
    }

    private void UpdateUIPosition() {
        EnemyHPbar.transform.position = hpBarPointAtEnemyHead.position;
        EnemyHPbar.transform.forward = mainCameraTransform.forward;
    }

    private void AutoHideUI() {
        if (showTimer <= 0 && !isAlwaysShow) {
            EnemyHPbar.SetActive(false);
        } else {
            showTimer -= Time.deltaTime;
        }
    }

    private void UpdateHealthBarUI(int currentHealth, int maxHealth) {
        currentHP.fillAmount = (float)currentHealth / maxHealth;

        if (currentHealth > 0) {
            EnemyHPbar.SetActive(true);
            showTimer = showTime;
        }

        changeHP.DOFillAmount(currentHP.fillAmount, changeTime).SetEase(Ease.OutQuad);
    }

    private void HideHealthBarUI() {
        enemyHPPool.ReturnToPool(EnemyHPbar);
    }

    // 使用 DOTween 替代协程进行血条填充动画。减少使用协程所带来的开销
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
