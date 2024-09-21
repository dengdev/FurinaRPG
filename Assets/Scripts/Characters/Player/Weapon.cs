using UnityEngine;

public class Weapon : MonoBehaviour
{
    private CharacterStats playerStats; // ��ҽ�ɫ������
    private Collider weaponCollider;

    void Awake()
    {
        weaponCollider = GetComponent<Collider>();
        if (weaponCollider != null)
        {
            weaponCollider.enabled = false; // ��ʼ״̬������ײ��
        }
    }

    // �������ù����ߵ�����
    public void SetAttacker(PlayerController player)
    {
        playerStats = player.GetplayerStats;
    }

    // �ڶ����¼��е��ã�����������ײ��
    public void EnableWeaponCollider()
    {
        if (weaponCollider != null)
        {
            weaponCollider.enabled = true;
            Debug.Log("����������ײ��");
        }
    }

    // �ڶ����¼��е��ã����ڽ�����ײ��
    public void DisableWeaponCollider()
    {
        if (weaponCollider != null)
        {
            weaponCollider.enabled = false;
            Debug.Log("����������ײ��");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // ����Ƿ��������ײ
        if (other.CompareTag("Enemy"))
        {
            CharacterStats enemyStats = other.GetComponent<CharacterStats>();
            if (enemyStats != null)
            {
                // ���㲢Ӧ���˺�
                enemyStats.TakeCharacterDamage(playerStats, enemyStats);
            }
        }
    }
}
