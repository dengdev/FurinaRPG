using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBarUI : MonoBehaviour {
    [Header("UI设置")]
    public string healthBarUIPrefabPath = "UI/HealthBarUI";
    public bool isUIAlwaysVisible;

    private float showUITime; 
    private float uIVisibleTime = 2.0f;
    private Transform healthBarPointAtEnemyHead; // 血条挂载点（在敌人头顶）
    private Transform uiBarTransform; // UI对象的Transform
    private Image healthSlider; // 血条填充条
    private Transform mainCameraTransform; // 主相机的Transform
    private CharacterStats enemyCurrentStats; // 敌人的状态信息

    [SerializeField] private GameObject healthUIPrefab; // 血条预制体


    private void Awake() {
        InitializeHealthBar();
    }

    private void OnEnable() {
        mainCameraTransform = Camera.main.transform;
        // 订阅血量变化事件
        if (enemyCurrentStats != null) {
            enemyCurrentStats.OnHealthChanged += UpdateHealthBarUI;
            enemyCurrentStats.OnDeath += HideHealthBarUI;
        }
    }

    private void OnDisable() {
        if (enemyCurrentStats != null) {
            enemyCurrentStats.OnHealthChanged -= UpdateHealthBarUI;
            enemyCurrentStats.OnDeath -= HideHealthBarUI;
        }
    }
    private void LateUpdate() {
        if (uiBarTransform != null) {
            UpdateUIPosition();
            AutoHideUI();
        }
    }

    private void InitializeHealthBar() {
        enemyCurrentStats = GetComponent<CharacterStats>();

        healthBarPointAtEnemyHead = transform.Find("HealthBar Point");
        if (healthBarPointAtEnemyHead == null) {
            Debug.LogError("未找到敌人头顶的 'HealthBar Point' 节点: " + gameObject.name);
            return;
        }

        if (healthUIPrefab == null) {
            healthUIPrefab = Resources.Load<GameObject>(healthBarUIPrefabPath);
            if (healthUIPrefab == null) {
                Debug.LogError($"未在路径 '{healthBarUIPrefabPath}' 找到血条预制体");
                return;
            }
        }

        foreach (Canvas canvas in FindObjectsOfType<Canvas>()) {
            if (canvas.renderMode == RenderMode.WorldSpace && uiBarTransform == null) {
                uiBarTransform = Instantiate(healthUIPrefab, canvas.transform).transform;
                healthSlider = uiBarTransform.GetChild(0).GetComponent<Image>();
                uiBarTransform.gameObject.SetActive(isUIAlwaysVisible);
            }
        }

        if (enemyCurrentStats != null) {
            UpdateHealthBarUI(enemyCurrentStats.CurrentHealth, enemyCurrentStats.MaxHealth);
        }
    }

    private void UpdateUIPosition() {
        uiBarTransform.position = healthBarPointAtEnemyHead.position;
        uiBarTransform.forward = -mainCameraTransform.forward; // 使血条始终面向玩家
    }

    private void AutoHideUI() {
        if (showUITime <= 0 && !isUIAlwaysVisible) {
            uiBarTransform.gameObject.SetActive(false);
        } else {
            showUITime -= Time.deltaTime;
        }
    }

    private void UpdateHealthBarUI(int currentHealth, int maxHealth) {
        if (uiBarTransform == null || healthSlider == null) {
            Debug.LogWarning("UI Bar或Health Slider未初始化");
            return;
        }

        // 更新血条显示
        float sliderPercent = (float)currentHealth / maxHealth;
        healthSlider.fillAmount = sliderPercent;

        // 如果当前血量大于0，显示血条
        if (currentHealth > 0) {
            uiBarTransform.gameObject.SetActive(true);
            showUITime = uIVisibleTime; // 重置显示时间
        }
    }
    private void HideHealthBarUI() {
        if (uiBarTransform != null) {
            Destroy(uiBarTransform.gameObject); // 销毁血条UI
        }
    }

}
