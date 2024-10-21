using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class CharacterStats : MonoBehaviour {
    [Header("角色数据")]
    public Attackdata_SO attackData;
    public Characters characterData;
    public PlayerData playerData;
    public EnemyData enemyData;

    [HideInInspector]
    public bool isCritical; // 是否暴击
    public bool isAttacking;

    private GameObject damageText;
    private Canvas worldSpaceCanvas;//红和白跳字
    private Canvas cameraSpaceCanvas;
    private Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
        damageText = ResourceManager.Instance.LoadResource<GameObject>("Prefabs/UI/DamageText");
        FindCanvas();
    }

    private void Start() {
        InitializeCharacterData();
    }

    public int MaxHealth { get => characterData?.maxHealth ?? 0; set => characterData.maxHealth = value; }
    public int CurrentHealth { get => characterData?.currentHealth ?? 0; set => characterData.currentHealth = value; }
    public int BaseDefence { get => characterData?.baseDefence ?? 0; set => characterData.baseDefence = value; }
    public int CurrentDefence { get => characterData?.currentDefence ?? 0; set => characterData.currentDefence = value; }


    private void InitializeCharacterData() {

        if (TryGetComponent<PlayerController>(out _)) {
            playerData = characterData as PlayerData;
            if (playerData == null) {
                Debug.LogError("玩家身上的数据为空，还没有加载数据");
            } else {
                GameObject.Find("PlayerHealthCanvas").transform.GetChild(0).gameObject.SetActive(true);
            }
        } else {
            enemyData = characterData as EnemyData;
        }

    }
    private void FindCanvas() {

        foreach (Canvas canvas in FindObjectsOfType<Canvas>()) {
            if (canvas.renderMode == RenderMode.WorldSpace) {
                worldSpaceCanvas = canvas;
                Debug.Log("找到世界画布");
            } else if (canvas.renderMode == RenderMode.ScreenSpaceCamera) {
                cameraSpaceCanvas = canvas;
                Debug.Log("找到相机画布");
            }
        }

        if (worldSpaceCanvas == null || cameraSpaceCanvas == null) {
            Debug.LogError("未找到合适的Canvas");
        }
    }

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
        return attackData == null ? 0 : Mathf.RoundToInt(UnityEngine.Random.Range(attackData.minDamage, attackData.maxDamage));
    }

    private int CalculateDamage(int damage, int defence) {
        return Mathf.Max(damage - defence, 1);
    }

    private void ApplyDamage(int damage, CharacterStats defender) {

        defender.CurrentHealth = Mathf.Max(defender.CurrentHealth - damage, 0);

        if (defender.CurrentHealth <= 0) {
            EventManager.Publish("EnemyDeath");
            if (defender.TryGetComponent<EnemyController>(out var enemyController)) {
                GameManager.Instance.playerData.AddExp(defender.enemyData.killPoint);
            }
        }

        // 使用ValueTuple<int, int>
        if (defender.characterData is PlayerData) {
            EventManager.Publish<(int, int)>("ChangePlayerHp", (defender.CurrentHealth, defender.MaxHealth));
            ShowDamageText_World(defender, damage);

        } else if(defender.characterData is EnemyData) {
            EventManager.Publish<(int, int)>("ChangeEnemyHp", (defender.CurrentHealth, defender.MaxHealth));
            ShowDamagePopupCameraSpace(defender, damage);
        }
    }

    private void ShowDamagePopupCameraSpace(CharacterStats defender, int damage) {
        if (GameManager.Instance.damageTextPool == null) {
            GameManager.Instance.damageTextPool = new ObjectPool(this.damageText, 5, 20, cameraSpaceCanvas.transform);
        }
        GameObject damageText = GameManager.Instance.damageTextPool.GetFromPool();
        RectTransform rectTransform = damageText.GetComponent<RectTransform>();
        TextMeshProUGUI _text = damageText.GetComponent<TextMeshProUGUI>();
        CanvasGroup canvasGroup = damageText.GetComponent<CanvasGroup>();

        Vector2 screenPosition = Camera.main.WorldToScreenPoint(defender.transform.position);
        screenPosition.y += 150f;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(cameraSpaceCanvas.GetComponent<RectTransform>(), screenPosition, cameraSpaceCanvas.worldCamera, out Vector2 anchorPosition);

        rectTransform.anchoredPosition = anchorPosition;
        rectTransform.localScale = Vector3.zero;
        canvasGroup.alpha = 1f; // 确保透明度为 1
        _text.text = damage.ToString();

        float randomXOffset = UnityEngine.Random.Range(-150f, 150f);
        Vector2 targetPosition = new Vector2(randomXOffset, 100f) + anchorPosition;

        rectTransform.DOScale(Vector3.one, 0.8f).SetEase(Ease.OutBack);
        rectTransform.DOAnchorPos(targetPosition, 2f).SetEase(Ease.OutQuad);
        canvasGroup.DOFade(0f, 1f).SetDelay(0.6f).OnComplete(() => {
            GameManager.Instance.damageTextPool.ReturnToPool(damageText);
        });
    }
    private void TriggerHitAnimation(CharacterStats defender) {

        if (defender.animator != null) {
            if (defender.TryGetComponent<PlayerController>(out var player)) {
                if (!player.NowIsHitState()) {
                    player.ChangeState(new HitState());
                }
            } else {
                defender.animator.SetTrigger("Hit");
            }
            StartCoroutine(ResetHitAnimation(defender));
        }
    }

    private IEnumerator ResetHitAnimation(CharacterStats defender) {
        yield return new WaitForSeconds(0.5f);

        if (defender.TryGetComponent<PlayerController>(out var player)) {
            player.ChangeState(new IdleState());
        } else {
            defender.animator.Play("Idle");
        }
    }

    private void ShowDamageText_World(CharacterStats defender, int damage) {
        Transform damageShow = Instantiate(Resources.Load<GameObject>("Prefabs/UI/DamageJumpShow"), worldSpaceCanvas.transform).transform;
        float randomRange = 0.8f;
        Vector3 randomOffset = new Vector3(
            UnityEngine.Random.Range(-randomRange, randomRange),
            UnityEngine.Random.Range(0.8f, 1.4f),
            UnityEngine.Random.Range(-randomRange, randomRange)
        );

        damageShow.position = defender.transform.position + randomOffset;
        damageShow.forward = Camera.main.transform.forward;
        damageShow.GetComponent<TextMeshProUGUI>().text = damage.ToString();

        StartCoroutine(DamageText_MoveAndFade_World(damageShow));
    }

    private IEnumerator DamageText_MoveAndFade_World(Transform damageShow) {
        float duration = 1.5f;
        float elapsed = 0f;
        float height = 0.5f;
        float distance = 2.5f;
        Vector3 startPos = damageShow.position;
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
