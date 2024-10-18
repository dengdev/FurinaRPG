using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class CharacterStats : MonoBehaviour {
    public event Action<int, int> OnHealthChanged;
    public event Action OnDeath;
    public event Action<int> OnGainExp;

    [Header("角色数据")]
    public Attackdata_SO attackData;
    public Characters characterData;
    public PlayerData playerData;
    public EnemyData enemyData;

    [HideInInspector]
    public bool isCritical; // 是否暴击
    public bool isAttacking;

    [Header("伤害跳字")]
    private GameObject damageText_World;
    private GameObject damgeText;

    private Canvas worldSpaceCanvas;
    private Canvas cameraSpaceCanvas;

    private Animator animator;
    private void Awake() {
        animator = GetComponent<Animator>();
        damgeText = ResourceManager.Instance.LoadResource<GameObject>("Prefabs/UI/DamageText");

        // 通过查找渲染模式找到血条挂载的画布。
        foreach (Canvas canvas in FindObjectsOfType<Canvas>()) {
            if (canvas.renderMode == RenderMode.WorldSpace) {
                worldSpaceCanvas = canvas;
            } else if (canvas.renderMode == RenderMode.ScreenSpaceCamera) {
                cameraSpaceCanvas = canvas;
            }
        }

        if (worldSpaceCanvas == null || cameraSpaceCanvas == null) {
            Debug.LogError("未找到合适的Canvas");
        }
    }

    private void Start() {
        if (transform.GetComponent<PlayerController>() != null) {
            playerData = (PlayerData)characterData;
            if (playerData != null) {
                GameObject.Find("PlayerHealth Canvas").transform.GetChild(0).gameObject.SetActive(true);
            } else {
                Debug.LogError("玩家身上的数据为空，还没有加载数据");
            }
        } else {
            enemyData = (EnemyData)characterData;
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
        ShowDamagePopupCameraSpace(defender, damage);

        defender.CurrentHealth = Mathf.Max(defender.CurrentHealth - damage, 0);
        if (defender.CurrentHealth < 0.01) {
            defender.OnDeath?.Invoke();
            if (defender.GetComponent<EnemyController>() != null) {
                PlayerGainExp(defender.enemyData.killPoint);
            }
        }
        OnHealthChanged?.Invoke(defender.CurrentHealth, defender.MaxHealth);
    }

    private void PlayerGainExp(int exp) {
        GameManager.Instance.playerStats.playerData.UpdateExp(exp);
        GameManager.Instance.playerStats.OnGainExp?.Invoke(exp);
    }


    private void ShowDamagePopupCameraSpace(CharacterStats defender, int damage) {
        if (GameManager.Instance.damageTextPool == null) {
            GameManager.Instance.damageTextPool = new ObjectPool(damgeText, 5, 20, cameraSpaceCanvas.transform);
        }

        GameObject damageText = GameManager.Instance.damageTextPool.GetFromPool();
        RectTransform rectTransform = damageText.GetComponent<RectTransform>();
        TextMeshProUGUI _text = damageText.GetComponent<TextMeshProUGUI>();
        CanvasGroup canvasGroup = damageText.GetComponent<CanvasGroup>();

        Vector2 screenPosition = Camera.main.WorldToScreenPoint(defender.transform.position);
        screenPosition.y += 150f;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(cameraSpaceCanvas.GetComponent<RectTransform>(), screenPosition, cameraSpaceCanvas.worldCamera, out Vector2 anchorPosition);

        if (rectTransform != null) {
            rectTransform.anchoredPosition = anchorPosition;
            rectTransform.localScale = Vector3.zero;
        }

        if (canvasGroup != null) {
            canvasGroup.alpha = 1f; // 确保透明度为 1
        }

        if (_text != null) {
            _text.text = damage.ToString();
        }


        float randomXOffset = UnityEngine.Random.Range(-150f, 150f);
        Vector2 targetPosition = new Vector2(randomXOffset, 100f)+ anchorPosition;

        rectTransform.DOScale(Vector3.one, 0.8f).SetEase(Ease.OutBack);
        rectTransform.DOAnchorPos(targetPosition, 2f).SetEase(Ease.OutQuad);

        canvasGroup.DOFade(0f, 1f).SetDelay(0.6f).OnComplete(() => {
            GameManager.Instance.damageTextPool.ReturnToPool(damageText);
        });
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


    private void ShowDamageText_World(CharacterStats defender, int damage) {

        if (damageText_World == null) {
            damageText_World = Resources.Load<GameObject>("UI/DamageJumpShow");
            if (damageText_World == null) {
                Debug.LogError($"未在路径 '{"UI/DamageJumpShow"}' 找到伤害跳字预制体");
                return;
            }
        }

        Transform damageShow = Instantiate(damageText_World, worldSpaceCanvas.transform).transform;
        float randomRange = 0.8f;
        Vector3 randomOffset = new Vector3(
            UnityEngine.Random.Range(-randomRange, randomRange),
            UnityEngine.Random.Range(0.8f, 1.4f),
            UnityEngine.Random.Range(-randomRange, randomRange)
        );

        damageShow.position = defender.transform.position + randomOffset;
        damageShow.forward = Camera.main.transform.forward;

        TextMeshProUGUI _text = damageShow.GetComponent<TextMeshProUGUI>();
        if (_text != null) {
            _text.text = damage.ToString();
        } else {
            Debug.LogError("Damage Popup没有找到TextMeshProUGUI组件");
        }

        StartCoroutine(DamageText_MoveAndFade_World(damageShow));
    }

    private IEnumerator DamageText_MoveAndFade_World(Transform damageShow) {
        float duration = 1.5f;
        float elapsed = 0f;

        Vector3 startPos = damageShow.position;

        float height = 0.5f;
        float distance = 2.5f;

        TextMeshProUGUI textComponent = damageShow.GetComponent<TextMeshProUGUI>();
        Color originalColor = textComponent.color;

        while (elapsed < duration) {
            float t = elapsed / duration;
            Vector3 targetPos = startPos + Vector3.right * distance * t + Vector3.up * height * Mathf.Sin(t * Mathf.PI);
            textComponent.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(1, 0, elapsed / duration));
            damageShow.position = targetPos;

            elapsed += Time.deltaTime;
            yield return null;
        }
        Destroy(damageShow.gameObject);
    }
    #endregion
}
