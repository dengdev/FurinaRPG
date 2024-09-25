using UnityEngine;

public class Weapon : MonoBehaviour {
    private Collider weaponCollider;

    void Awake() {
        weaponCollider = GetComponent<Collider>();
        if (weaponCollider != null) {
            weaponCollider.enabled = false;
        }
    }

    public void EnableWeaponCollider() {
        if (weaponCollider != null) {
            weaponCollider.enabled = true;
        }
    }

    public void DisableWeaponCollider() {
        if (weaponCollider != null) {
            weaponCollider.enabled = false;
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
