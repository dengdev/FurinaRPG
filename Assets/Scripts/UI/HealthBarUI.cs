using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ��ʾ���˵�Ѫ��UI
/// </summary>
public class HealthBarUI : MonoBehaviour
{
    // Ѫ��UI��Prefab
    public GameObject HealthUIPrefab;
    // Ѫ��UI�ڳ����е�ê�㣨ͨ���ǽ�ɫͷ��λ�ã�
    public Transform barPoint;
    // ����Ѫ����Image���
    Image healthSlider;
    // Ѫ��UI��Transform
    Transform UIbar;
    // ���Transform��������LateUpdate��ʹѪ���������
    Transform mainCameraTransform; 
    // Ѫ���Ƿ�ʼ�տɼ�
    public bool alwaysVisible;
    // Ѫ�����ܻ���ɼ���ʱ��
    public float visibleTime;
    // ʣ��ɼ�ʱ��
    private float timeLeft;

    // ��ǰ���˵�������Ϣ����Ѫ����
    CharacterStats currentStats;

    private void Awake()
    {
        currentStats = GetComponent<CharacterStats>();

        currentStats.UpdateHealthBarOnAttack += UpdateHealthBar;
    }

    // ����������ʱ���ҵ�UI����ʼ��Ѫ��
    private void OnEnable()
    {
        // ��ȡ�������Transform
        mainCameraTransform = Camera.main.transform;
        // ���������е�Canvas���ҵ�WorldSpaceģʽ��Canvas
        foreach (Canvas canvas in FindObjectsOfType<Canvas>())
        {
            if(canvas.renderMode==RenderMode.WorldSpace)
            {
              UIbar=  Instantiate(HealthUIPrefab, canvas.transform).transform;
                healthSlider=UIbar.GetChild(0).GetComponent<Image>();
                UIbar.gameObject.SetActive(alwaysVisible);
            }
        }
    }

    /// <summary>
    /// ����Ѫ����ʾ
    /// </summary>
    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (currentHealth <= 0) // ���Ѫ��Ϊ0������Ѫ��UI
            Destroy(UIbar.gameObject);
        UIbar.gameObject.SetActive(true);
        timeLeft = visibleTime; // ����Ѫ���ɼ�ʱ��
        // ���㵱ǰѪ���ٷֱȣ�������Ѫ���������
        float sliderPercent =(float) currentHealth / maxHealth;
        healthSlider.fillAmount = sliderPercent;
    }

    // ��LateUpdate�и���UI��λ�úͳ���ʹ�䳯�����
    private void LateUpdate()
    {
        if (UIbar != null)
        {
            // Ѫ��UI�����ɫ��barPointλ��
            UIbar.position = barPoint.position;
            UIbar.forward = -mainCameraTransform.forward; // Ѫ��UI�������
            if (timeLeft<=0&&!alwaysVisible) // ���Ѫ���Ŀɼ�ʱ�䵽���Ҳ���Ҫʼ�տɼ���������Ѫ��
                UIbar.gameObject.SetActive(false);
            else
                timeLeft-= Time.deltaTime;
        }
    }
}
