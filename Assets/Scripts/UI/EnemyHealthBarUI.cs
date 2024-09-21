using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBarUI : MonoBehaviour {
    [Header("UI����")]
    public string healthBarUIPrefabPath = "UI/HealthBarUI"; // Ѫ��Ԥ����·��
    public bool isUIAlwaysVisible; // Ѫ���Ƿ�ʼ�տɼ�

    private float showUITime; // UI��ʾ��ʣ��ʱ��
    private float uIVisibleTime = 2.0f; // UI�Զ�����ǰ����ʾʱ��
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

        // ��ȡ����ͷ���Ĺ��ص�
        healthBarPointAtEnemyHead = transform.Find("HealthBar Point");
        if (healthBarPointAtEnemyHead == null) {
            Debug.LogError("δ�ҵ�����ͷ���� 'HealthBar Point' �ڵ�: " + gameObject.name);
            return;
        }

        // ���ز�ʵ����UI
        if (healthUIPrefab == null) {
            healthUIPrefab = Resources.Load<GameObject>(healthBarUIPrefabPath);
            if (healthUIPrefab == null) {
                Debug.LogError($"δ��·�� '{healthBarUIPrefabPath}' �ҵ�Ѫ��Ԥ����");
                return;
            }
        }

        // ��WorldSpace��Canvas���ҵ�һ�����ص�
        foreach (Canvas canvas in FindObjectsOfType<Canvas>()) {
            if (canvas.renderMode == RenderMode.WorldSpace && uiBarTransform == null) {
                uiBarTransform = Instantiate(healthUIPrefab, canvas.transform).transform;
                healthSlider = uiBarTransform.GetChild(0).GetComponent<Image>();
                uiBarTransform.gameObject.SetActive(isUIAlwaysVisible);
            }
        }

        // ���³�ʼѪ����ʾ
        if (enemyCurrentStats != null) {
            UpdateHealthBarUI(enemyCurrentStats.CurrentHealth, enemyCurrentStats.MaxHealth);
        }
    }

    // ����Ѫ��λ��
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
    // ����Ѫ��UI
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
