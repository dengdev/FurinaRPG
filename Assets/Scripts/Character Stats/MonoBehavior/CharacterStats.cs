using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour {

    public event Action<int, int> OnHealthChanged; // 血量变化事件
    public event Action OnDeath;// 角色死亡事件   

    [Header("角色数据")]
    public CharacterData_SO characterTemplateData;
    public CharacterData_SO characterData;
    public Attackdata_SO attackData;

    [HideInInspector]
    public bool isCritical; // 是否暴击
    public int previousState; // 受击前的状态

    private Animator animator;
    private bool isHitAnimation; // 是否在受击动画中

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

    public void TakeDamage(int damage, CharacterStats defender) {
        if (defender == null) { Debug.LogWarning("Defender is null"); return; }

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
        defender.OnHealthChanged?.Invoke(defender.CurrentHealth, defender.MaxHealth);
        if (defender.CurrentHealth <= 0) { 
        CheckCharacterDeath(GameManager.Instance.playerStats, defender);
    }

    }

    private void CheckCharacterDeath(CharacterStats attacker, CharacterStats defender) {
        if (defender.CurrentHealth <= 0 && defender.characterData != null) {
            if (attacker != null) {
                attacker.characterData.UpdateExp(defender.characterData.killPoint); // 玩家获得击杀经验
            }
            defender.OnDeath?.Invoke();
        }
    }

    private void TriggerHitAnimation(CharacterStats defender) {
        if (defender.isHitAnimation) return;

        defender.isHitAnimation = true;
        if (defender.animator != null) {
            defender.previousState = defender.animator.GetCurrentAnimatorStateInfo(0).shortNameHash;
            defender.animator.SetTrigger("Hit");
            StartCoroutine(WaitForHitAnimation(defender));
        }
    }

    private IEnumerator WaitForHitAnimation(CharacterStats defender) {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(stateInfo.length);

        defender.isHitAnimation = false;
        defender.animator.Play(defender.previousState);
    }
    #endregion
}
