// InteractionDroppedWeapon.cs
using UnityEngine;

public class InteractionDroppedWeapon : MonoBehaviour, IInteractionHandler
{
    public string interactionMessage;
    public WeaponData weaponData; // Dati dell'arma che può essere raccolta

    private WeaponSystem weaponSystem;

    private void Start()
    {
        weaponSystem = FindObjectOfType<WeaponSystem>();
        if (weaponSystem == null)
        {
            Debug.LogError("WeaponSystem non trovato nella scena.");
        }
    }

    public void HandleInteraction(InteractableObject interactableObject)
    {
        PickupWeapon();
    }

    private void PickupWeapon()
    {
        if (weaponSystem != null && weaponData != null)
        {
            weaponSystem.PickupWeapon(weaponData);
            Debug.Log(interactionMessage);
            Destroy(gameObject); // Distrugge il prefab dell'arma a terra
        }
        else
        {
            Debug.LogError("WeaponSystem o WeaponData non assegnati.");
        }
    }
}
