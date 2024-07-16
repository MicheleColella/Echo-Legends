using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    public WeaponData weaponData; // Riferimento allo ScriptableObject WeaponData
    private bool canDealDamage = false;

    private void OnTriggerStay(Collider other)
    {
        if (canDealDamage && other.CompareTag("Enemy"))
        {
            float damage = Random.Range(weaponData.damageRange.x, weaponData.damageRange.y);
            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage((int)damage);
                Debug.Log("Hit enemy: " + other.name + " with damage: " + damage);

                // Disabilitare il danno per evitare colpi multipli nello stesso frame
                canDealDamage = false;
            }
        }
    }

    // Questo metodo sarà chiamato dall'animazione per abilitare il danno
    public void EnableDamage()
    {
        canDealDamage = true;
    }

    // Questo metodo sarà chiamato dall'animazione per disabilitare il danno
    public void DisableDamage()
    {
        canDealDamage = false;
    }
}
