using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public GameObject HealthUIPrefab;
    public Transform barPoint;
    Image healthSlider;
    Transform UIbar;
    new Transform camera;
    public bool alwaysVisible;
    public float visibleTime;
    private float timeLeft;

    CharacterStats currentStats;
    private void Awake()
    {
        currentStats = GetComponent<CharacterStats>();

        currentStats.UpdateHealthBarOnAttack += UpdateHealthBar;
    }

    private void OnEnable()
    {
        camera = Camera.main.transform;

        foreach(Canvas canvas in FindObjectsOfType<Canvas>())
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
    /// 更新血条显示
    /// </summary>
    /// <param name="currentHealth">当前血量</param>
    /// <param name="maxHealth">最大血量</param>
    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (currentHealth <= 0)
            Destroy(UIbar.gameObject);
        UIbar.gameObject.SetActive(true);
        timeLeft = visibleTime;
        float sliderPercent=(float) currentHealth / maxHealth;
        healthSlider.fillAmount = sliderPercent;
    }

    private void LateUpdate()
    {
        if (UIbar != null)
        {
            UIbar.position = barPoint.position;
            UIbar.forward = -camera.forward;
            if(timeLeft<=0&&!alwaysVisible)
                UIbar.gameObject.SetActive(false);
            else
                timeLeft-= Time.deltaTime;
        }
    }
}
