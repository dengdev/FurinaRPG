using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterStats : MonoBehaviour {

    public event Action<int, int> OnHealthChanged;
    public event Action OnDeath;
    public event Action<int> OnGainExp;

    [Header("角色数据")]
    public CharacterData_SO characterTemplateData;
    public Attackdata_SO attackData;
    public CharacterData_SO characterData;

    private Animator animator;

    [HideInInspector]
    public bool isCritical; // 是否暴击
    public bool isAttacking; // 是否在攻击中

    [Header("伤害跳字")]
    public string damagePopupPrefabPath = "UI/DamageJumpShow";
    [SerializeField] private GameObject damagePopupPrefab;
    private Canvas damageShowCanvas;
    private Transform damageShow;//实例化出来的跳字


    private void Awake() {
        if (characterTemplateData != null) {
            characterData = Instantiate(characterTemplateData);
        }

        animator = GetComponent<Animator>();

        if (damagePopupPrefab == null) {
            damagePopupPrefab = Resources.Load<GameObject>(damagePopupPrefabPath);
            if (damagePopupPrefab == null) {
                Debug.LogError($"未在路径 '{damagePopupPrefabPath}' 找到伤害跳字预制体");
                return;
            }
        }

        // 通过查找渲染模式找到血条挂载的画布。
        foreach (Canvas canvas in FindObjectsOfType<Canvas>()) {
            if (canvas.renderMode == RenderMode.WorldSpace) {
                damageShowCanvas = canvas;
            }
        }
    }

    #region 角色属性
    public int MaxHealth {
        get { return characterData != null ? characterData.maxHealth : 0; }
        set { characterData.maxHealth = value; }
    }

    public int CurrentHealth {
        get { return characterData != null ? characterData.currentHealth : 0; }
        set { characterData.currentHealth = value; }
    }

    public int BaseDefence {
        get { return characterData != null ? characterData.baseDefence : 0; }
        set { characterData.baseDefence = value; }
    }

    public int CurrentDefence {
        get { return characterData != null ? characterData.currentDefence : 0; }
        set { characterData.currentDefence = value; }
    }
    #endregion
   
    #region 角色战斗逻辑
    public void TakeCharacterDamage(CharacterStats attacker, CharacterStats defender) {
        if (attacker == null || defender == null) return;

        int baseDamage = CalculateDamage(attacker.CurrentDamage(), defender.CurrentDefence);
        if (isCritical) { baseDamage = (int)(baseDamage * attackData.criticalMultiplier); }

        ApplyDamage(baseDamage, defender);
        TriggerHitAnimation(defender);
    }

    public void TakeRockDamage(int damage, CharacterStats defender) {
        int finalDamage = CalculateDamage(damage, defender.CurrentDefence);
        ApplyDamage(finalDamage, defender);
        TriggerHitAnimation(defender);
    }

    private int CurrentDamage() {
        if (attackData == null) return 0;
        return Mathf.RoundToInt(UnityEngine.Random.Range(attackData.minDamage, attackData.maxDamage));
    }

    private int CalculateDamage(int damage, int defence) {
        return Mathf.Max(damage - defence, 1);
    }

    private void ApplyDamage(int damage, CharacterStats defender) {

        ShowDamagePopup(defender, damage);
        defender.CurrentHealth = Mathf.Max(defender.CurrentHealth - damage, 0);
        if (defender.CurrentHealth < 0.01) {
            defender.OnDeath?.Invoke();
            PlayerGainExp(defender.characterData.killPoint);
        }
        OnHealthChanged?.Invoke(defender.CurrentHealth, defender.MaxHealth);
    }

    private void PlayerGainExp(int exp) {
        GameManager.Instance.playerStats.characterData.UpdateExp(exp); // 玩家获得击杀经验
        GameManager.Instance.playerStats.OnGainExp?.Invoke(exp);
    }

    private void ShowDamagePopup(CharacterStats defender, int damage) {
        damageShow = Instantiate(damagePopupPrefab, damageShowCanvas.transform).transform;

        float randomRange = 0.8f;
        // 生成随机偏移
        Vector3 randomOffset = new Vector3(
            UnityEngine.Random.Range(-randomRange, randomRange),
            UnityEngine.Random.Range(0.8f, 1.4f), // 只在Y轴上产生正偏移
            UnityEngine.Random.Range(-randomRange, randomRange)
        );

        damageShow.position = defender.transform.position + randomOffset;
        damageShow.forward = Camera.main.transform.forward; // 使数字始终面向玩家

        TextMeshProUGUI _text = damageShow.GetComponent<TextMeshProUGUI>();
        if (_text != null) {
            _text.text = damage.ToString();
        } else {
            Debug.LogError("Damage Popup没有找到TextMeshProUGUI组件");
        }

        // 启动协程处理移动和消失
        StartCoroutine(MoveAndFade(damageShow));
    }

    private void TriggerHitAnimation(CharacterStats defender) {
        if (defender.animator != null) {

            if (defender.GetComponent<PlayerController>() != null) {

                if (!defender.GetComponent<PlayerController>().NowIsHitState()) {
                    defender.transform.GetComponent<PlayerController>().ChangeState(new HitState());
                }
            } else {
                defender.animator.SetTrigger("Hit");
            }
            StartCoroutine(ResetHitAnimation(defender));
        }
    }

    private IEnumerator ResetHitAnimation(CharacterStats defender) {

        float hitAnimationLength = 0.5f;
        yield return new WaitForSeconds(hitAnimationLength);

        if (defender.GetComponent<PlayerController>() != null) {
            defender.transform.GetComponent<PlayerController>().ChangeState(new IdleState());
        } else {
            defender.animator.Play("Idle");

        }
    }

    private IEnumerator MoveAndFade(Transform damageShow) {
        float duration = 1.5f; // 总持续时间
        float elapsed = 0f;

        // 起始位置
        Vector3 startPos = damageShow.position;

        // 控制水平和垂直偏移
        float height = 0.5f; // 最大高度
        float distance = 2.5f; // 水平移动的距离


        // 淡出效果
        TextMeshProUGUI textComponent = damageShow.GetComponent<TextMeshProUGUI>();
        Color originalColor = textComponent.color;

        // 移动
        while (elapsed < duration) {
            // 计算抛物线运动
            float t = elapsed / duration;
            Vector3 targetPos = startPos + Vector3.right * distance * t + Vector3.up * height * Mathf.Sin(t * Mathf.PI); // 通过Sin函数模拟抛物线
            textComponent.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(1, 0, elapsed / duration));
            damageShow.position = targetPos;

            elapsed += Time.deltaTime;
            yield return null; // 等待下一帧
        }
        // 销毁对象
        Destroy(damageShow.gameObject);
    }
    #endregion
}
