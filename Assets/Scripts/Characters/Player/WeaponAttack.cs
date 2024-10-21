using UnityEngine;

public class WeaponAttack : MonoBehaviour {
    private Collider weaponCollider;
    public TrailRenderer trailRenderer;

    void Awake() {
        weaponCollider = GetComponent<Collider>();

        if (weaponCollider != null) {
            weaponCollider.enabled = false;
            trailRenderer.emitting = false; // ��ʼ������β

        }
    }

    public void EnableWeaponCollider() {
        if (weaponCollider != null) {
            weaponCollider.enabled = true;
            trailRenderer.emitting = true;
        }

    }

    public void DisableWeaponCollider() {
        if (weaponCollider != null) {
            weaponCollider.enabled = false;
            trailRenderer.emitting = false;
        }
    }

    public bool CheckCollision(out Collider target) {
        target = null;
        Collider[] hits = Physics.OverlapBox(weaponCollider.bounds.center, weaponCollider.bounds.extents, Quaternion.identity);
        foreach (Collider hit in hits) {
            if (hit.CompareTag("Enemy")||hit.CompareTag("Attackable")) {
                target = hit;
                return true;
            }
        }
        return false;
    }
}
