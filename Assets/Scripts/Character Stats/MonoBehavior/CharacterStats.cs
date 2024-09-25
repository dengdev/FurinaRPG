using System;
using System.Collections;
using UnityEngine;

public class CharacterStats : MonoBehaviour {

    public event Action<int, int> OnHealthChanged;
    public event Action OnDeath;

    [Header("角色数据")]
    public CharacterData_SO characterTemplateData;
    public Attackdata_SO attackData;
    public CharacterData_SO characterData;

    private Animator animator;

    [HideInInspector]
    public bool isCritical; // 是否暴击
    public bool isAttacking; // 是否在攻击中

    private bool isHitAnimation; // 是否在受击动画中
    private int previousState;

    private void Awake() {
        if (characterTemplateData != null)
            characterData = Instantiate(characterTemplateData);
        animator = GetComponent<Animator>();
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
        defender.CurrentHealth = Mathf.Max(defender.CurrentHealth - damage, 0);
        if (defender.CurrentHealth <= 0) {
            defender.OnDeath?.Invoke();
            PlayerGainExp(defender);
        }
        OnHealthChanged?.Invoke(defender.CurrentHealth, defender.MaxHealth);
    }

    private void PlayerGainExp(CharacterStats defender) {
        GameManager.Instance.playerStats.characterData.UpdateExp(defender.characterData.killPoint); // 玩家获得击杀经验
    }

    private void TriggerHitAnimation(CharacterStats defender) {
        if (defender.isHitAnimation) return;

        defender.isHitAnimation = true;
        if (defender.animator != null) {
            defender.previousState = defender.animator.GetCurrentAnimatorStateInfo(0).shortNameHash;
            defender.animator.SetTrigger("Hit");
            StartCoroutine(ResetHitAnimation(defender));
        }
    }

    private IEnumerator ResetHitAnimation(CharacterStats defender) {
        yield return new WaitForSeconds(defender.animator.GetCurrentAnimatorStateInfo(0).length);
        defender.isHitAnimation = false;
        if (!defender.isAttacking) {
            defender.animator.Play(defender.previousState);
        }
    }
    #endregion
}
