using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class CharacterStats : MonoBehaviour {



    public event Action<int, int> OnHealthChanged;
    public event Action OnDeath;
    public event Action<int> OnGainExp;

    [Header("��ɫ����")]
    public CharacterData_SO characterTemplateData;
    public Attackdata_SO attackData;
    public CharacterData_SO characterData;

    private Animator animator;

    [HideInInspector]
    public bool isCritical; // �Ƿ񱩻�
    public bool isAttacking; // �Ƿ��ڹ�����

    [Header("�˺�����")]
    public string damagePopupPrefabPath = "UI/DamageJumpShow";
    [SerializeField] private GameObject damagePopupPrefab;
    private Canvas damageShowCanvas;
    private Transform damageShow;//ʵ��������������


    private void Awake() {
        if (characterTemplateData != null) {
            characterData = Instantiate(characterTemplateData);
        }

        animator = GetComponent<Animator>();

        if (damagePopupPrefab == null) {
            damagePopupPrefab = Resources.Load<GameObject>(damagePopupPrefabPath);
            if (damagePopupPrefab == null) {
                Debug.LogError($"δ��·�� '{damagePopupPrefabPath}' �ҵ��˺�����Ԥ����");
                return;
            }
        }

        // ͨ��������Ⱦģʽ�ҵ�Ѫ�����صĻ�����
        foreach (Canvas canvas in FindObjectsOfType<Canvas>()) {
            if (canvas.renderMode == RenderMode.WorldSpace) {
                damageShowCanvas = canvas;
            }
        }
    }

    #region ��ɫ����
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


    private void LateUpdate() {

    }

    #region ��ɫս���߼�
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
        GameManager.Instance.playerStats.characterData.UpdateExp(exp); // ��һ�û�ɱ����
        GameManager.Instance.playerStats.OnGainExp?.Invoke(exp);
    }

    private void ShowDamagePopup(CharacterStats defender, int damage) {
        damageShow = Instantiate(damagePopupPrefab, damageShowCanvas.transform).transform;

        float randomRange = 0.8f;
        // �������ƫ��
        Vector3 randomOffset = new Vector3(
            UnityEngine.Random.Range(-randomRange, randomRange),
            UnityEngine.Random.Range(0.8f, 1.4f), // ֻ��Y���ϲ�����ƫ��
            UnityEngine.Random.Range(-randomRange, randomRange)
        );


        damageShow.position = defender.transform.position + randomOffset;
        damageShow.forward = Camera.main.transform.forward; // ʹ����ʼ���������

        TextMeshProUGUI _text = damageShow.GetComponent<TextMeshProUGUI>();
        if (_text != null) {
            _text.text = damage.ToString();
        } else {
            Debug.LogError("Damage Popupû���ҵ�TextMeshProUGUI���");
        }

        // ����Э�̴����ƶ�����ʧ
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
        float duration = 1.5f; // �ܳ���ʱ��
        float elapsed = 0f;

        // ��ʼλ��
        Vector3 startPos = damageShow.position;

        // ����ˮƽ�ʹ�ֱƫ��
        float height = 0.5f; // ���߶�
        float distance = 2.5f; // ˮƽ�ƶ��ľ���


        // ����Ч��
        TextMeshProUGUI textComponent = damageShow.GetComponent<TextMeshProUGUI>();
        Color originalColor = textComponent.color;

        // �ƶ�
        while (elapsed < duration) {
            // �����������˶�
            float t = elapsed / duration;
            Vector3 targetPos = startPos + Vector3.right * distance * t + Vector3.up * height * Mathf.Sin(t * Mathf.PI); // ͨ��Sin����ģ��������
            textComponent.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(1, 0, elapsed / duration));
            damageShow.position = targetPos;

            elapsed += Time.deltaTime;
            yield return null; // �ȴ���һ֡
        }
        // ���ٶ���
        Destroy(damageShow.gameObject);
    }

    #endregion
}
