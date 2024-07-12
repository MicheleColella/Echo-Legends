using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    private bool canDealDamage = false;

    private void OnTriggerEnter(Collider other)
    {
        if (canDealDamage && other.CompareTag("Enemy"))
        {
            Debug.Log("Hit enemy: " + other.name);
            // Implementa qui la logica di danno al nemico
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
