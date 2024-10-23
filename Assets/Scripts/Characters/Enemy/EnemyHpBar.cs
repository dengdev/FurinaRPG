using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class EnemyHpBar : MonoBehaviour {
    [Header("敌人血条设置")]
    public bool isAlwaysShow;
    public float showTime = 2.0f;
    public float changeTime = 0.9f;

    private float showTimer;
    private Transform headPoint;
    private GameObject enemyHpBar;
    private Image virtualHp;
    private Image realHp;
    private CharacterStats enemStats;

    private void OnEnable() {
        EventManager.Subscribe("EnemyDeath", EnemyDeath);
        EventManager.Subscribe<(int, int)>("ChangeEnemyHp", UpdateHealthBarUI);

        enemStats = GetComponent<CharacterStats>();
        headPoint = transform.Find("HealthBar Point");

        if (headPoint == null) {
            Debug.LogError("未找到敌人头顶的 'HealthBar Point' 节点: " + gameObject.name);
            return;
        }
        InitializeHealthBar();

        if (enemStats.enemyData != null) {
            realHp.fillAmount = enemStats.CurrentHealth / enemStats.MaxHealth;
            virtualHp.fillAmount = realHp.fillAmount;
        }
    }

    private void OnDisable() {
        EventManager.Unsubscribe("EnemyDeath", EnemyDeath);
        EventManager.Unsubscribe<(int, int)>("ChangeEnemyHp", UpdateHealthBarUI);
    }

    private void LateUpdate() {
        if (enemyHpBar != null) {
            enemyHpBar.transform.position = headPoint.position;
            enemyHpBar.transform.forward = Camera.main.transform.forward;
            AutoHideHpBar();
        }
    }

    private void InitializeHealthBar() {
        GameObject healthUiPrefab = ResourceManager.Instance.LoadResource<GameObject>("Prefabs/UI/Effect/EnemyHPBar");
        // 找到敌人血条画布
        foreach (Canvas canvas in FindObjectsOfType<Canvas>()) {
            if (canvas.renderMode == RenderMode.WorldSpace) {
                if (GameManager.Instance.enemyHpPool == null) {
                    GameManager.Instance.enemyHpPool = new ObjectPool(healthUiPrefab, 5, 20, canvas.transform);
                }
                enemyHpBar = GameManager.Instance.enemyHpPool.GetFromPool();
                virtualHp = enemyHpBar.transform.GetChild(0).GetComponent<Image>();
                realHp = enemyHpBar.transform.GetChild(1).GetComponent<Image>();
                enemyHpBar.SetActive(isAlwaysShow);
            }
        }
    }

    private void AutoHideHpBar() {

        if (showTimer <= 0 && !isAlwaysShow) {
            enemyHpBar.SetActive(false);
        } else {
            showTimer -= Time.deltaTime;
        }
    }

    private void UpdateHealthBarUI((int currentHealth, int maxHealth) healthData) {
        float healthPercent = (float)enemStats.CurrentHealth / enemStats.MaxHealth;

        if (enemStats.CurrentHealth > 0) {
            enemyHpBar.SetActive(true);
            showTimer = showTime;
        }
        // 血条变化,变化图片的填充值
        realHp.fillAmount = healthPercent;
        virtualHp.DOFillAmount(healthPercent, changeTime).SetEase(Ease.OutQuad);
    }

    private void EnemyDeath() {
        if (enemStats.CurrentHealth <= 0) {
           GameManager.Instance.enemyHpPool.ReturnToPool(enemyHpBar);
        }
    }
}
