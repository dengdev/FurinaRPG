using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBarUI : MonoBehaviour {
    [Header("UI����")]
    public string healthBarUIPrefabPath = "UI/HealthBarUI";
    public bool isUIAlwaysVisible;

    private float showUITime; 
    private float uIVisibleTime = 2.0f;
    private Transform healthBarPointAtEnemyHead; // Ѫ�����ص㣨�ڵ���ͷ����
    private Transform uiBarTransform; // UI�����Transform
    private Image healthSlider; // Ѫ�������
    private Transform mainCameraTransform; // �������Transform
    private CharacterStats enemyCurrentStats; // ���˵�״̬��Ϣ

    [SerializeField] private GameObject healthUIPrefab; // Ѫ��Ԥ����


    private void Awake() {
        InitializeHealthBar();
    }

    private void OnEnable() {
        mainCameraTransform = Camera.main.transform;
        // ����Ѫ���仯�¼�
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
            Debug.LogError("δ�ҵ�����ͷ���� 'HealthBar Point' �ڵ�: " + gameObject.name);
            return;
        }

        if (healthUIPrefab == null) {
            healthUIPrefab = Resources.Load<GameObject>(healthBarUIPrefabPath);
            if (healthUIPrefab == null) {
                Debug.LogError($"δ��·�� '{healthBarUIPrefabPath}' �ҵ�Ѫ��Ԥ����");
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
        uiBarTransform.forward = -mainCameraTransform.forward; // ʹѪ��ʼ���������
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
            Debug.LogWarning("UI Bar��Health Sliderδ��ʼ��");
            return;
        }

        // ����Ѫ����ʾ
        float sliderPercent = (float)currentHealth / maxHealth;
        healthSlider.fillAmount = sliderPercent;

        // �����ǰѪ������0����ʾѪ��
        if (currentHealth > 0) {
            uiBarTransform.gameObject.SetActive(true);
            showUITime = uIVisibleTime; // ������ʾʱ��
        }
    }
    private void HideHealthBarUI() {
        if (uiBarTransform != null) {
            Destroy(uiBarTransform.gameObject); // ����Ѫ��UI
        }
    }

}
