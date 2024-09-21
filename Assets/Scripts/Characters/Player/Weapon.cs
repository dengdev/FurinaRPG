using UnityEngine;

public class Weapon : MonoBehaviour
{
    private CharacterStats playerStats; // 玩家角色的属性
    private Collider weaponCollider;

    void Awake()
    {
        weaponCollider = GetComponent<Collider>();
        if (weaponCollider != null)
        {
            weaponCollider.enabled = false; // 初始状态禁用碰撞器
        }
    }

    // 用于设置攻击者的引用
    public void SetAttacker(PlayerController player)
    {
        playerStats = player.GetplayerStats;
    }

    // 在动画事件中调用，用于启用碰撞器
    public void EnableWeaponCollider()
    {
        if (weaponCollider != null)
        {
            weaponCollider.enabled = true;
            Debug.Log("启用武器碰撞器");
        }
    }

    // 在动画事件中调用，用于禁用碰撞器
    public void DisableWeaponCollider()
    {
        if (weaponCollider != null)
        {
            weaponCollider.enabled = false;
            Debug.Log("禁用武器碰撞器");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 检查是否与敌人碰撞
        if (other.CompareTag("Enemy"))
        {
            CharacterStats enemyStats = other.GetComponent<CharacterStats>();
            if (enemyStats != null)
            {
                // 计算并应用伤害
                enemyStats.TakeCharacterDamage(playerStats, enemyStats);
            }
        }
    }
}
