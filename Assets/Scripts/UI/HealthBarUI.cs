using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 显示敌人的血量UI
/// </summary>
public class HealthBarUI : MonoBehaviour
{
    // 血条UI的Prefab
    public GameObject HealthUIPrefab;
    // 血条UI在场景中的锚点（通常是角色头顶位置）
    public Transform barPoint;
    // 引用血条的Image组件
    Image healthSlider;
    // 血条UI的Transform
    Transform UIbar;
    // 相机Transform，用于在LateUpdate中使血条朝向相机
    Transform mainCameraTransform; 
    // 血条是否始终可见
    public bool alwaysVisible;
    // 血条在受击后可见的时间
    public float visibleTime;
    // 剩余可见时间
    private float timeLeft;

    // 当前敌人的属性信息（如血量）
    CharacterStats currentStats;

    private void Awake()
    {
        currentStats = GetComponent<CharacterStats>();

        currentStats.UpdateHealthBarOnAttack += UpdateHealthBar;
    }

    // 当物体启用时，找到UI并初始化血条
    private void OnEnable()
    {
        // 获取主相机的Transform
        mainCameraTransform = Camera.main.transform;
        // 遍历场景中的Canvas，找到WorldSpace模式的Canvas
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
    /// 更新血条显示
    /// </summary>
    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (currentHealth <= 0) // 如果血量为0，销毁血条UI
            Destroy(UIbar.gameObject);
        UIbar.gameObject.SetActive(true);
        timeLeft = visibleTime; // 重置血条可见时间
        // 计算当前血量百分比，并更新血条的填充量
        float sliderPercent =(float) currentHealth / maxHealth;
        healthSlider.fillAmount = sliderPercent;
    }

    // 在LateUpdate中更新UI的位置和朝向，使其朝向相机
    private void LateUpdate()
    {
        if (UIbar != null)
        {
            // 血条UI跟随角色的barPoint位置
            UIbar.position = barPoint.position;
            UIbar.forward = -mainCameraTransform.forward; // 血条UI朝向相机
            if (timeLeft<=0&&!alwaysVisible) // 如果血条的可见时间到了且不需要始终可见，则隐藏血条
                UIbar.gameObject.SetActive(false);
            else
                timeLeft-= Time.deltaTime;
        }
    }
}
